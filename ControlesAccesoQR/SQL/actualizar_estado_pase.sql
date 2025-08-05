USE [vhs];
GO

IF OBJECT_ID('[vhs].[actualizar_estado_pase]', 'P') IS NOT NULL
    DROP PROCEDURE [vhs].[actualizar_estado_pase];
GO

CREATE PROCEDURE [vhs].[actualizar_estado_pase]
    @NumeroPase VARCHAR(50),
    @Estado     VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM vhs.PasePuerta WHERE NumeroPase = @NumeroPase)
    BEGIN
        SELECT 
            CAST(NULL AS INT) AS PasePuertaID,
            CAST(NULL AS VARCHAR(50)) AS NumeroPase,
            CAST(NULL AS VARCHAR(1)) AS Estado,
            CAST(NULL AS DATETIME) AS FechaCreacion,
            CAST(NULL AS DATETIME) AS FechaActualizacion
        WHERE 1 = 0;
        RETURN;
    END

    UPDATE vhs.PasePuerta
    SET Estado = @Estado,
        FechaActualizacion = GETDATE()
    WHERE NumeroPase = @NumeroPase;

    SELECT 
        PasePuertaID,
        NumeroPase,
        Estado,
        FechaCreacion,
        FechaActualizacion
    FROM vhs.PasePuerta
    WHERE NumeroPase = @NumeroPase;
END
GO
