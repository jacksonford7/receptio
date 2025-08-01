USE [RECEPTIO]
GO
/****** Object:  StoredProcedure [dbo].[mb_get_valida_smdt]    Script Date: 17/9/2018 16:04:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[mb_get_valida_smdt]
(  
	@ref_num nvarchar(25)
)
As
Set Implicit_Transactions Off --apagar transaccion implicita
Set Quoted_Identifier Off --apagar contador de quotas
Set Nocount On --apagar mensajes de pantalla
Set DateFormat ymd --establecer formato de fecha para este procedmiento
--SET ANSI_WARNINGS Off

begin try  
        declare @LN_rowcount INT
	   BEGIN
		 select T.[CODIGO_TRANSACCION]
		 FROM [ecuapass].[dbo].[ECU_TRANSACCION] T
		   inner join N4Middleware.[dbo].[VBS_T_PASE_PUERTA] (nolock) as p on T.CODIGO_CARGA=p.ID_CARGA
		 where CODIGO_EDOC_TRANSACCION='023' and
		      T.ESTADO='A' AND
		      P.ID_PASE= @ref_num
			  SELECT @LN_rowcount=@@ROWCOUNT
		  IF   @LN_rowcount > 0 
			 BEGIN
				select 0 as [code], '' as [message]
		        return;
		 	 END
	   END
	select    -1 as [code], 'NO EXISTE UNIDAD AUTORIZADA' as [message] 
end try  
begin catch  
     select  -2 as [code], ERROR_MESSAGE() as [message] 
end catch;