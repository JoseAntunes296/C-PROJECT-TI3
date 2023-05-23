
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/04/2023 11:38:36
-- Generated from EDMX file: C:\Users\joser\OneDrive\Ambiente de Trabalho\project\Project\Project\Models\ProjectModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [project];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__projectAs__proje__3E52440B]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[projectAssignment] DROP CONSTRAINT [FK__projectAs__proje__3E52440B];
GO
IF OBJECT_ID(N'[dbo].[FK__projectAs__userI__628FA481]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[projectAssignment] DROP CONSTRAINT [FK__projectAs__userI__628FA481];
GO
IF OBJECT_ID(N'[dbo].[FK__tasks__projectId__3B75D760]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tasks] DROP CONSTRAINT [FK__tasks__projectId__3B75D760];
GO
IF OBJECT_ID(N'[dbo].[FK__tasks__UserTaskI__6E01572D]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[tasks] DROP CONSTRAINT [FK__tasks__UserTaskI__6E01572D];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[projectAssignment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[projectAssignment];
GO
IF OBJECT_ID(N'[dbo].[projects]', 'U') IS NOT NULL
    DROP TABLE [dbo].[projects];
GO
IF OBJECT_ID(N'[dbo].[tasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tasks];
GO
IF OBJECT_ID(N'[dbo].[users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'projectAssignments'
CREATE TABLE [dbo].[projectAssignments] (
    [IdProjectAssigment] int IDENTITY(1,1) NOT NULL,
    [userId] int  NOT NULL,
    [projectId] int  NOT NULL,
    [status] varchar(10)  NOT NULL
);
GO

-- Creating table 'projects'
CREATE TABLE [dbo].[projects] (
    [IdProject] int IDENTITY(1,1) NOT NULL,
    [name] nchar(50)  NOT NULL,
    [description] varchar(255)  NOT NULL,
    [startDate] datetime  NOT NULL,
    [endDate] datetime  NOT NULL
);
GO

-- Creating table 'tasks'
CREATE TABLE [dbo].[tasks] (
    [IdTask] int IDENTITY(1,1) NOT NULL,
    [name] varchar(50)  NOT NULL,
    [description] varchar(144)  NOT NULL,
    [deadline] datetime  NULL,
    [status] nchar(10)  NOT NULL,
    [projectId] int  NOT NULL,
    [UserTaskId] int  NOT NULL
);
GO

-- Creating table 'users'
CREATE TABLE [dbo].[users] (
    [IdUser] int IDENTITY(1,1) NOT NULL,
    [username] nchar(10)  NOT NULL,
    [email] varchar(50)  NOT NULL,
    [password] varchar(50)  NOT NULL,
    [profileImg] varchar(50)  NULL,
    [administrator] int  NOT NULL,
    [token] varchar(100)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [IdProjectAssigment] in table 'projectAssignments'
ALTER TABLE [dbo].[projectAssignments]
ADD CONSTRAINT [PK_projectAssignments]
    PRIMARY KEY CLUSTERED ([IdProjectAssigment] ASC);
GO

-- Creating primary key on [IdProject] in table 'projects'
ALTER TABLE [dbo].[projects]
ADD CONSTRAINT [PK_projects]
    PRIMARY KEY CLUSTERED ([IdProject] ASC);
GO

-- Creating primary key on [IdTask] in table 'tasks'
ALTER TABLE [dbo].[tasks]
ADD CONSTRAINT [PK_tasks]
    PRIMARY KEY CLUSTERED ([IdTask] ASC);
GO

-- Creating primary key on [IdUser] in table 'users'
ALTER TABLE [dbo].[users]
ADD CONSTRAINT [PK_users]
    PRIMARY KEY CLUSTERED ([IdUser] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [projectId] in table 'projectAssignments'
ALTER TABLE [dbo].[projectAssignments]
ADD CONSTRAINT [FK__projectAs__proje__3E52440B]
    FOREIGN KEY ([projectId])
    REFERENCES [dbo].[projects]
        ([IdProject])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__projectAs__proje__3E52440B'
CREATE INDEX [IX_FK__projectAs__proje__3E52440B]
ON [dbo].[projectAssignments]
    ([projectId]);
GO

-- Creating foreign key on [userId] in table 'projectAssignments'
ALTER TABLE [dbo].[projectAssignments]
ADD CONSTRAINT [FK__projectAs__userI__3F466844]
    FOREIGN KEY ([userId])
    REFERENCES [dbo].[users]
        ([IdUser])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__projectAs__userI__3F466844'
CREATE INDEX [IX_FK__projectAs__userI__3F466844]
ON [dbo].[projectAssignments]
    ([userId]);
GO

-- Creating foreign key on [projectId] in table 'tasks'
ALTER TABLE [dbo].[tasks]
ADD CONSTRAINT [FK__tasks__projectId__3B75D760]
    FOREIGN KEY ([projectId])
    REFERENCES [dbo].[projects]
        ([IdProject])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__tasks__projectId__3B75D760'
CREATE INDEX [IX_FK__tasks__projectId__3B75D760]
ON [dbo].[tasks]
    ([projectId]);
GO

-- Creating foreign key on [UserTaskId] in table 'tasks'
ALTER TABLE [dbo].[tasks]
ADD CONSTRAINT [FK__tasks__UserTaskI__6E01572D]
    FOREIGN KEY ([UserTaskId])
    REFERENCES [dbo].[users]
        ([IdUser])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__tasks__UserTaskI__6E01572D'
CREATE INDEX [IX_FK__tasks__UserTaskI__6E01572D]
ON [dbo].[tasks]
    ([UserTaskId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------