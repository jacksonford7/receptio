USE [RECEPTIO]
GO
/****** Object:  StoredProcedure [dbo].[mb_get_ecuapass_message_pass]    Script Date: 11/10/2018 9:54:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[mb_get_ecuapass_message_pass]
(  
       @ref_num nvarchar(25)
)
As
Set Implicit_Transactions Off --apagar transaccion implicita
Set Quoted_Identifier Off --apagar contador de quotas
Set Nocount On --apagar mensajes de pantalla
Set DateFormat ymd --establecer formato de fecha para este procedmiento
SET ANSI_WARNINGS Off


set @ref_num = ltrim(rtrim(@ref_num));

if @ref_num is null or len(@ref_num) <= 3
begin
return;
end

/*
cruza con la tabla ecu_solicitud_trasnacción-> retorna el campo NUMERO_ENTREGA
->Si la trasnaccion esta ESTADO:
N-->ESTE CAMPO ES VACIO, YA QUE AUN NO SE ENVIA ASI QUE NO TIENES ERRORES
S->SE ACABA DE ENVIAR AUN NO TIENES MENSAJES DE ERROR, PERO OJO PUEDE QUE ECUAPASS PORTAL SE LA COMA
E->ERROR DE NUESTRO LADO EN DATOS O APLICACIÓN
R->EN ESTE CASO SI TIENES NUMERO_ENTREGA Y AHI TE VAS A LA TABLA
A->APROBADO NO HAY NADA QUE RETORNA

*/


begin try  
   
SELECT [CODIGO_TRANSACCION],CODIGO_SOLICITUD_TRANSACCION,
[NUMERO_ERROR] id
      ,[ERROR_DESCRIPTION] mensaje
  FROM [ecuapass].[dbo].[ECU_TRANSACCION] (nolock)
  inner join  [ecuapass].[dbo].[ECU_RESP_ERRORES](nolock)  on [CODIGO_TRANSACCION]=[REF_DOCUMENT]
  inner join N4Middleware.[dbo].[VBS_T_PASE_PUERTA] (nolock) as p on CODIGO_CARGA=p.ID_CARGA
  WHERE P.ID_PASE= @ref_num


end try  
begin catch  
     select  ERROR_MESSAGE() 
end catch;
