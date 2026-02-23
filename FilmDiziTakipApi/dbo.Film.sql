CREATE TABLE [dbo].[Film] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Ad]          NVARCHAR (MAX)  NOT NULL,
    [Aciklama]    NVARCHAR (MAX)  NOT NULL,
    [YayınTarihi] DATETIME2 (7)   NOT NULL,
    [Yonetmen]    NVARCHAR (MAX)  NOT NULL,
    [CikisTarihi] NVARCHAR (MAX)  NOT NULL,
    [Puan]        DECIMAL (18, 2) NOT NULL,
    [KategoriId]  INT             NOT NULL,
    CONSTRAINT [PK_Film] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Film_Kategori_KategoriId] FOREIGN KEY ([KategoriId]) REFERENCES [dbo].[Kategori] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Film_KategoriId]
    ON [dbo].[Film]([KategoriId] ASC);

