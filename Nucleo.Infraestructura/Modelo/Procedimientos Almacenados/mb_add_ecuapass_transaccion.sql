USE [RECEPTIO]
GO
/****** Object:  StoredProcedure [dbo].[mb_add_ecuapass_transaccion]    Script Date: 11/10/2018 9:54:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[mb_add_ecuapass_transaccion]
(  
	        @CODIGO_CARGA bigint
           ,@CODIGO_TIPO_CARGA varchar(5)='CNTR' -->CNTR-BRBK
           ,@OBJETO_SOLICITA varchar(30)
           ,@USUARIO_SOLICITA char(30)
           ,@CONTENEDOR varchar(15)= null
           ,@MRN varchar(30)
           ,@MSN varchar(5)
           ,@HSN varchar(5)
           ,@NUMERO_ENTREGA varchar(30)
           ,@COMENTARIOS varchar(250)='TRANSACCION MANUAL'
)
As
Set Implicit_Transactions Off --apagar transaccion implicita
Set Quoted_Identifier Off --apagar contador de quotas
Set Nocount On --apagar mensajes de pantalla
Set DateFormat ymd --establecer formato de fecha para este procedmiento
SET ANSI_WARNINGS Off
declare @LV_CODIGO_CARGA bigint

select @LV_CODIGO_CARGA=p.ID_CARGA
from N4Middleware.[dbo].[VBS_T_PASE_PUERTA] (nolock) as p 
where id_pase=@CODIGO_CARGA


declare  @REENVIO numeric(2,0)=-99;

if(@CONTENEDOR is not null)
begin
	select @REENVIO = COUNT(1) from [ecuapass].[dbo].[ECU_SOLICITUD_TRANSACCION](nolock)
	where CODIGO_EDOC_SOLICITUD = '023' and MRN =@MRN
	and MSN= @MSN and HSN= @HSN and CONTENEDOR = @CONTENEDOR
	set @REENVIO = ISNULL(@reenvio,-50);
end




begin try  
   
   INSERT INTO [ecuapass].[dbo].[ECU_SOLICITUD_TRANSACCION]
           ([CODIGO_CARGA]-->
           ,[CODIGO_TIPO_CARGA]-->CNTR-BRBK
           ,[CODIGO_TRAFICO]-->IMPRT
           ,[CODIGO_EDOC_SOLICITUD] -->'023'
           ,[REENVIO]-->
           ,[OBJETO_SOLICITA]
           ,[USUARIO_SOLICITA]
           ,[FECHA_SOLICITUD]
           ,[PRIORIDAD]
           ,[CONTENEDOR]
           ,[ESTADO]
           ,[MRN]
           ,[MSN]
           ,[HSN]
           ,[NUMERO_ENTREGA]
           ,[COMENTARIOS]
           ,[FECHA_PROCESAMIENTO]
           ,[FECHA_ENVIO_SOLICITANTE]
           ,[FECHA_RESPUESTA]
           ,[ESTADO_ENVIO_SOLICITANTE]
           ,[COMENTARIOS_ENVIO]
           ,[FECHA_ENVIO_TRANSACCION]
          )
     VALUES
           (
		    @LV_CODIGO_CARGA 
           ,@CODIGO_TIPO_CARGA 
           ,'IMPRT'
           ,'023' 
           ,@REENVIO 
           ,@OBJETO_SOLICITA 
           ,@USUARIO_SOLICITA 
           ,getdate()
           ,0
           ,@CONTENEDOR 
           ,'A'
           ,@MRN 
           ,@MSN 
           ,@HSN 
           ,@NUMERO_ENTREGA 
           ,@COMENTARIOS 
           ,GETDATE()
           ,GETDATE()
           ,GETDATE()
           ,'A'
           ,'TRANSACCION MANUAL DESDE TD'
           ,GETDATE()
		   )


	select   scope_identity() as [code], 'Ok' as [message] 
end try  
begin catch  
     select  cast(0 as decimal(18,0)) as [code], ERROR_MESSAGE() as [message] --Fallo x Timeout truncado un otro error en el proceso
end catch;



 
