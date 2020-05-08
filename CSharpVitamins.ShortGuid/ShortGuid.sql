IF EXISTS (SELECT TOP 1 1 FROM sysobjects WHERE id = object_id( N'[<Owner,varchar,dbo>].[DecodeShortGuid]' ) AND OBJECTPROPERTY( id, N'IsScalarFunction' ) = 1)
	DROP FUNCTION [<Owner,varchar,dbo>].[DecodeShortGuid]
GO

IF EXISTS (SELECT TOP 1 1 FROM sysobjects WHERE id = object_id( N'[<Owner,varchar,dbo>].[EncodeShortGuid]' ) AND OBJECTPROPERTY( id, N'IsScalarFunction' ) = 1)
	DROP FUNCTION [<Owner,varchar,dbo>].[EncodeShortGuid]
GO
/*
Converts the @uuid uniqueidentifier to a shortguid style string.
*/
CREATE FUNCTION [<Owner,varchar,dbo>].[EncodeShortGuid](
	@uuid uniqueidentifier
)
RETURNS varchar(22) AS
BEGIN
	DECLARE @bin binary(16)
	SET @bin = CAST(@uuid as binary(16))

	RETURN SUBSTRING(
		REPLACE(
			REPLACE(
				CAST(N'' as xml).value(
					'xs:base64Binary(sql:variable("@bin"))',
					'VARCHAR(24)'
				),
				'/', '_'
			),
			'+', '-'
		),
		1, 22
	)
END
GO
/*
Converts the shortguid style string in @data to a uniqueidentifier.
*/
CREATE FUNCTION [<Owner,varchar,dbo>].[DecodeShortGuid](
	@data varchar(22)
)
RETURNS uniqueidentifier AS
BEGIN
	DECLARE @b64String varchar(24),
		@bin binary(16)

	SET @b64String = REPLACE(REPLACE(@data, '_', '/'), '-', '+') + '=='

	SET @bin = CAST(N'' as xml).value(
		'xs:base64Binary(sql:variable("@b64String") )',
		'binary(16)'
	)

	RETURN CAST(@bin as uniqueidentifier)
END
GO
