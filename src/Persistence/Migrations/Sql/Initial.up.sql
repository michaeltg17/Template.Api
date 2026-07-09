CREATE TABLE Users
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
    [Name] NVARCHAR(50) NOT NULL,
    Email NVARCHAR(50) NOT NULL,

    CreatedBy BIGINT NOT NULL,
    CreatedOn DATETIME2(0) NOT NULL,
    ModifiedBy BIGINT NULL,
    ModifiedOn DATETIME2(0) NULL,

    IsTest BIT NOT NULL DEFAULT(0),

    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_Users_Guid UNIQUE ([Guid] ASC)
);
GO

SET IDENTITY_INSERT Users ON;
INSERT INTO Users ([Id], [Name], Email, CreatedBy, CreatedOn)
VALUES (1, 'System', 'system@system.com', 1, GETDATE());
SET IDENTITY_INSERT Users OFF;
GO

ALTER TABLE Users
    ADD CONSTRAINT FK_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id);
GO

ALTER TABLE Users
    ADD CONSTRAINT FK_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users (Id);
GO

CREATE TABLE ImageResolutions
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
    [Name] NVARCHAR(25) NOT NULL,

    CreatedBy BIGINT NOT NULL,
    CreatedOn DATETIME2(0) NOT NULL,
    ModifiedBy BIGINT NULL,
    ModifiedOn DATETIME2(0) NULL,

    IsTest BIT NOT NULL DEFAULT(0),

    CONSTRAINT PK_ImageResolutions PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_ImageResolutions_Guid UNIQUE ([Guid] ASC),
    CONSTRAINT FK_ImageResolutions_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id),
    CONSTRAINT FK_ImageResolutions_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users (Id)
);
GO

INSERT INTO ImageResolutions ([Name], CreatedBy, CreatedOn) VALUES
    ('Original', 1, GETDATE()),
    ('Hd', 1, GETDATE()),
    ('FullHd', 1, GETDATE());
GO

CREATE TABLE ImageTypes
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
    Abbreviation NVARCHAR(4) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,

    CreatedBy BIGINT NOT NULL,
    CreatedOn DATETIME2(0) NOT NULL,
    ModifiedBy BIGINT NULL,
    ModifiedOn DATETIME2(0) NULL,

    IsTest BIT NOT NULL DEFAULT(0),

    CONSTRAINT PK_ImageTypes PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_ImageTypes_Guid UNIQUE ([Guid] ASC),
    CONSTRAINT FK_ImageTypes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id),
    CONSTRAINT FK_ImageTypes_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users (Id)
);
GO

INSERT INTO ImageTypes ([Name], Abbreviation, CreatedBy, CreatedOn) VALUES
    ('Graphics Interchange Format', 'GIF', 1, GETDATE()),
    ('Joint Photographic Expert Group', 'JPEG', 1, GETDATE()),
    ('Portable Network Graphics', 'PNG', 1, GETDATE());
GO

CREATE TABLE ImageGroups
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
    [Name] NVARCHAR(50) NOT NULL,
    [Type] BIGINT NOT NULL,

    CreatedBy BIGINT NOT NULL,
    CreatedOn DATETIME2(0) NOT NULL,
    ModifiedBy BIGINT NULL,
    ModifiedOn DATETIME2(0) NULL,

    IsTest BIT NOT NULL DEFAULT(0),

    CONSTRAINT PK_ImageGroups PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_ImageGroups_Guid UNIQUE ([Guid] ASC),
    CONSTRAINT FK_ImageGroups_Type FOREIGN KEY ([Type]) REFERENCES ImageTypes (Id),
    CONSTRAINT FK_ImageGroups_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id),
    CONSTRAINT FK_ImageGroups_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users (Id)
);
GO

CREATE TABLE ImageFileExtensions
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
    ImageType BIGINT NOT NULL,
    FileExtension NVARCHAR(4) NOT NULL,

    CreatedBy BIGINT NOT NULL,
    CreatedOn DATETIME2(0) NOT NULL,
    ModifiedBy BIGINT NULL,
    ModifiedOn DATETIME2(0) NULL,

    IsTest BIT NOT NULL DEFAULT(0),

    CONSTRAINT PK_ImageFileExtensions PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_ImageFileExtensions_Guid UNIQUE ([Guid] ASC),
    CONSTRAINT FK_ImageFileExtensions_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id),
    CONSTRAINT FK_ImageFileExtensions_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users (Id),
    CONSTRAINT FK_ImageFileExtensions_ImageType FOREIGN KEY (ImageType) REFERENCES ImageTypes (Id)
);
GO

INSERT INTO ImageFileExtensions (ImageType, FileExtension, CreatedBy, CreatedOn) VALUES
    (1, 'gif', 1, GETDATE()),
    (2, 'jpeg', 1, GETDATE()),
    (2, 'jpg', 1, GETDATE()),
    (3, 'png', 1, GETDATE());
GO

CREATE TABLE Images
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
    [Url] NVARCHAR(250) NOT NULL,
    Resolution BIGINT NOT NULL,
    [Group] BIGINT NOT NULL,

    CreatedBy BIGINT NOT NULL,
    CreatedOn DATETIME2(0) NOT NULL,
    ModifiedBy BIGINT NULL,
    ModifiedOn DATETIME2(0) NULL,

    IsTest BIT NOT NULL DEFAULT(0),

    CONSTRAINT PK_Images PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT UQ_Images_Guid UNIQUE ([Guid] ASC),
    CONSTRAINT FK_Images_Resolution FOREIGN KEY (Resolution) REFERENCES ImageResolutions (Id),
    CONSTRAINT FK_Images_Group FOREIGN KEY ([Group]) REFERENCES ImageGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Images_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id),
    CONSTRAINT FK_Images_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users (Id)
);
GO