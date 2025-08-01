USE [RECEPTIO]
GO
/****** Object:  StoredProcedure [dbo].[mb_get_gkey_cont]    Script Date: 11/10/2018 13:25:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[mb_get_gkey_cont]
(  
                    @lv_CONTENEDOR varchar(50)
)
As
Set Implicit_Transactions Off --apagar transaccion implicita
Set Quoted_Identifier Off --apagar contador de quotas
Set Nocount OFF --apagar mensajes de pantalla
Set DateFormat ymd --establecer formato de fecha para este procedmiento
SET ANSI_WARNINGS Off

DECLARE @lv_GKEY BIGINT

EXEC CGNDB02.N5.[DBO].FNA_FUN_CONTAINERS_GKEY @lv_CONTENEDOR,@lv_GKEY OUTPUT

SELECT @lv_GKEY as 'GKEY'

GO
