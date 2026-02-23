CREATE TABLE [dbo].[Dizi] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [Ad]             NVARCHAR (MAX)  NOT NULL,
    [Aciklama]       NVARCHAR (MAX)  NOT NULL,
    [YayınTarihi]    DATETIME2 (7)   NOT NULL,
    [Yonetmen]       NVARCHAR (MAX)  NOT NULL,
    [SezonSayisi]    INT             NOT NULL,
    [Puan]           DECIMAL (18, 2) NOT NULL,
    [KategoriId]     INT             NOT NULL,
    [YuklenmeTarihi] DATETIME        NOT NULL,
    CONSTRAINT [PK_Dizi] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Dizi_Kategori_KategoriId] FOREIGN KEY ([KategoriId]) REFERENCES [dbo].[Kategori] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Dizi_KategoriId]
    ON [dbo].[Dizi]([KategoriId] ASC);

