USE [RECEPTIO]
GO
/****** Object:  StoredProcedure [dbo].[mb_get_status_n4]    Script Date: 19/12/2018 9:04:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[mb_get_status_n4]
(  
	@PRE_GATE numeric(10,0)
)
As
Set Implicit_Transactions Off --apagar transaccion implicita
Set Quoted_Identifier Off --apagar contador de quotas
Set Nocount On --apagar mensajes de pantalla
Set DateFormat ymd --establecer formato de fecha para este procedmiento
SET ANSI_WARNINGS Off

DECLARE @STATUS_GOS VARCHAR(20),
        @CHANGED_GOS DATETIME,
		@CREATED  DATETIME,
		@ln_PRE_GATE numeric(10)=@PRE_GATE*-1

EXEC CGNDB02.N5.DBO.mb_STATUS_GOS @gkey=@ln_PRE_GATE,@STATUS=@STATUS_GOS OUTPUT,@FECHA=@CHANGED_GOS OUTPUT


SELECT @STATUS_GOS
 

