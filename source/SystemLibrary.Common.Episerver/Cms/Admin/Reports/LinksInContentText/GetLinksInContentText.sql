-- https://krompaco.nu/2022/10/identify-external-embedded-resources-in-cms-content/

IF OBJECT_ID('dbo.SystemLibraryCommonOptimizelyGetLinksInContentText') IS NOT NULL
  DROP FUNCTION SystemLibraryCommonOptimizelyGetLinksInContentText
GO

CREATE FUNCTION dbo.SystemLibraryCommonOptimizelyGetLinksInContentText (@Tekstas NVARCHAR(MAX))
RETURNS @Data TABLE(Url NVARCHAR(MAX))
AS
BEGIN
	DECLARE @FirstIndexOfChar INT,
			@LastIndexOfChar INT,
			@LengthOfStringBetweenChars INT,
			@String NVARCHAR(MAX)

	SET @FirstIndexOfChar = CHARINDEX('//', @Tekstas, 0)

	WHILE @FirstIndexOfChar > 0
	BEGIN

		SET @String = ''
		SET @LastIndexOfChar = CHARINDEX('/', @Tekstas,@FirstIndexOfChar+7)
		SET @LengthOfStringBetweenChars = @LastIndexOfChar - @FirstIndexOfChar + 1

		SET @String = SUBSTRING(@Tekstas, @FirstIndexOfChar, @LengthOfStringBetweenChars)
		INSERT INTO @Data (Url) VALUES (@String);

		SET @Tekstas = SUBSTRING(@Tekstas, @LastIndexOfChar, LEN(@Tekstas))
		SET @FirstIndexOfChar = CHARINDEX('//', @Tekstas, 0)
	END

	RETURN
END

GO

BEGIN

	DECLARE @external INT 
	SET @external = @isExternal@

	SELECT * FROM (
		SELECT fkContentID, LongString, Link.Url as Url
		FROM tblContentProperty
		OUTER APPLY dbo.SystemLibraryCommonOptimizelyGetLinksInText(LongString) as Link
		WHERE LongString IS NOT NULL
		AND LongStringLength > 4
		AND LongString LIKE '% src=%'
		) as t1
	WHERE t1.fkContentID > 0
	AND (@external = -1 OR (@external = 1 AND t1.Url IS NOT NULL) OR (@external = 0 AND t1.Url IS NULL))

END