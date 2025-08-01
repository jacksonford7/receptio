USE [RECEPTIO]
GO
/****** Object:  StoredProcedure [dbo].[mb_set_estate_smdt_transaccion]    Script Date: 11/10/2018 9:55:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[mb_set_estate_smdt_transaccion]
(  
	@key varchar(50),  @user varchar(100), @causa varchar(200)
)
As
Set Implicit_Transactions Off --apagar transaccion implicita
Set Quoted_Identifier Off --apagar contador de quotas
Set Nocount On --apagar mensajes de pantalla
Set DateFormat ymd --establecer formato de fecha para este procedmiento
SET ANSI_WARNINGS Off

set @user = LTRIM(rtrim(ISNULL(@user,'NULO')));
set @causa = LTRIM(rtrim(ISNULL(@causa,'NULO')));
declare @c tinyint=0;
begin transaction update_smdt
begin try 

update
ecuapass.dbo.ECU_SOLICITUD_TRANSACCION
set ESTADO = 'A' , COMENTARIOS = @causa, USUARIO_SOLICITA= @user, OBJETO_SOLICITA ='TD->gate' 
, FECHA_RESPUESTA = GETDATE()
where NUMERO_ENTREGA= @key and CODIGO_EDOC_SOLICITUD = '023'
set @c = @@ROWCOUNT;

update
ecuapass.dbo.ECU_TRANSACCION
set ESTADO='A', COMENTARIOS = @causa, FECHA_RESPUESTA = GETDATE()
where CODIGO_EDOC_TRANSACCION = '023'and CODIGO_TRANSACCION = @key

set @c += @@ROWCOUNT;

commit transaction update_smdt
end try
begin catch
	if( xact_state() <> 0 )
	begin 
	rollback transaction update_smdt
	end
end catch
select @c


