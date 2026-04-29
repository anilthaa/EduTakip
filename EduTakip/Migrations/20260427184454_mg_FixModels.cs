using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg_FixModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SinifOgrenciler tablosu hiç yoksa sıfırdan oluştur
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SinifOgrenciler')
                BEGIN
                    CREATE TABLE [SinifOgrenciler] (
                        [ID] int NOT NULL IDENTITY(1,1),
                        [Sinif_ID] int NOT NULL,
                        [Ogrenci_ID] int NOT NULL,
                        [KatilmaTarihi] datetime2 NOT NULL,
                        [CreUser] int NOT NULL,
                        [CreDate] datetime2 NOT NULL,
                        [UpdUser] int NULL,
                        [UpdDate] datetime2 NULL,
                        [DelUser] int NULL,
                        [DelDate] datetime2 NULL,
                        CONSTRAINT [PK_SinifOgrenciler] PRIMARY KEY ([ID]),
                        CONSTRAINT [FK_SinifOgrenciler_Ogrenciler_Ogrenci_ID] FOREIGN KEY ([Ogrenci_ID]) REFERENCES [Ogrenciler] ([ID]) ON DELETE NO ACTION,
                        CONSTRAINT [FK_SinifOgrenciler_Siniflar_Sinif_ID] FOREIGN KEY ([Sinif_ID]) REFERENCES [Siniflar] ([ID]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_SinifOgrenciler_Ogrenci_ID] ON [SinifOgrenciler] ([Ogrenci_ID]);
                    CREATE INDEX [IX_SinifOgrenciler_Sinif_ID] ON [SinifOgrenciler] ([Sinif_ID]);
                END
                ELSE
                BEGIN
                    -- Tablo var ama eski yapıda: FK'leri ve indexleri düşür, eski sütunları kaldır
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SinifOgrenciler_Ogrenciler_OgrenciID')
                        ALTER TABLE [SinifOgrenciler] DROP CONSTRAINT [FK_SinifOgrenciler_Ogrenciler_OgrenciID];
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SinifOgrenciler_Siniflar_SinifID')
                        ALTER TABLE [SinifOgrenciler] DROP CONSTRAINT [FK_SinifOgrenciler_Siniflar_SinifID];
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinifOgrenciler_OgrenciID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                        DROP INDEX [IX_SinifOgrenciler_OgrenciID] ON [SinifOgrenciler];
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinifOgrenciler_SinifID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                        DROP INDEX [IX_SinifOgrenciler_SinifID] ON [SinifOgrenciler];
                    IF EXISTS (SELECT * FROM sys.columns WHERE name = 'OgrenciID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                    BEGIN
                        DECLARE @df1 NVARCHAR(256);
                        SELECT @df1 = d.name FROM sys.default_constraints d
                            JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
                            WHERE c.object_id = OBJECT_ID('SinifOgrenciler') AND c.name = 'OgrenciID';
                        IF @df1 IS NOT NULL EXEC('ALTER TABLE [SinifOgrenciler] DROP CONSTRAINT [' + @df1 + ']');
                        ALTER TABLE [SinifOgrenciler] DROP COLUMN [OgrenciID];
                    END
                    IF EXISTS (SELECT * FROM sys.columns WHERE name = 'SinifID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                    BEGIN
                        DECLARE @df2 NVARCHAR(256);
                        SELECT @df2 = d.name FROM sys.default_constraints d
                            JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
                            WHERE c.object_id = OBJECT_ID('SinifOgrenciler') AND c.name = 'SinifID';
                        IF @df2 IS NOT NULL EXEC('ALTER TABLE [SinifOgrenciler] DROP CONSTRAINT [' + @df2 + ']');
                        ALTER TABLE [SinifOgrenciler] DROP COLUMN [SinifID];
                    END
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinifOgrenciler_Ogrenci_ID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                        CREATE INDEX [IX_SinifOgrenciler_Ogrenci_ID] ON [SinifOgrenciler] ([Ogrenci_ID]);
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinifOgrenciler_Sinif_ID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                        CREATE INDEX [IX_SinifOgrenciler_Sinif_ID] ON [SinifOgrenciler] ([Sinif_ID]);
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SinifOgrenciler_Ogrenciler_Ogrenci_ID')
                        ALTER TABLE [SinifOgrenciler] ADD CONSTRAINT [FK_SinifOgrenciler_Ogrenciler_Ogrenci_ID] FOREIGN KEY ([Ogrenci_ID]) REFERENCES [Ogrenciler] ([ID]) ON DELETE NO ACTION;
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SinifOgrenciler_Siniflar_Sinif_ID')
                        ALTER TABLE [SinifOgrenciler] ADD CONSTRAINT [FK_SinifOgrenciler_Siniflar_Sinif_ID] FOREIGN KEY ([Sinif_ID]) REFERENCES [Siniflar] ([ID]) ON DELETE CASCADE;
                END
            ");

            // DersProgramlari tablosundaki eski SinifID sütununu temizle
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DersProgramlari_Siniflar_SinifID')
                    ALTER TABLE [DersProgramlari] DROP CONSTRAINT [FK_DersProgramlari_Siniflar_SinifID];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DersProgramlari_SinifID' AND object_id = OBJECT_ID('DersProgramlari'))
                    DROP INDEX [IX_DersProgramlari_SinifID] ON [DersProgramlari];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'SinifID' AND object_id = OBJECT_ID('DersProgramlari'))
                BEGIN
                    DECLARE @dfName NVARCHAR(256);
                    SELECT @dfName = d.name FROM sys.default_constraints d
                        JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
                        WHERE c.object_id = OBJECT_ID('DersProgramlari') AND c.name = 'SinifID';
                    IF @dfName IS NOT NULL
                        EXEC('ALTER TABLE [DersProgramlari] DROP CONSTRAINT [' + @dfName + ']');
                    ALTER TABLE [DersProgramlari] DROP COLUMN [SinifID];
                END
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DersProgramlari_Sinif_ID' AND object_id = OBJECT_ID('DersProgramlari'))
                    CREATE INDEX [IX_DersProgramlari_Sinif_ID] ON [DersProgramlari] ([Sinif_ID]);
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DersProgramlari_Siniflar_Sinif_ID')
                    ALTER TABLE [DersProgramlari] ADD CONSTRAINT [FK_DersProgramlari_Siniflar_Sinif_ID] FOREIGN KEY ([Sinif_ID]) REFERENCES [Siniflar] ([ID]) ON DELETE NO ACTION;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SinifOgrenciler_Ogrenciler_Ogrenci_ID')
                    ALTER TABLE [SinifOgrenciler] DROP CONSTRAINT [FK_SinifOgrenciler_Ogrenciler_Ogrenci_ID];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SinifOgrenciler_Siniflar_Sinif_ID')
                    ALTER TABLE [SinifOgrenciler] DROP CONSTRAINT [FK_SinifOgrenciler_Siniflar_Sinif_ID];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinifOgrenciler_Ogrenci_ID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                    DROP INDEX [IX_SinifOgrenciler_Ogrenci_ID] ON [SinifOgrenciler];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SinifOgrenciler_Sinif_ID' AND object_id = OBJECT_ID('SinifOgrenciler'))
                    DROP INDEX [IX_SinifOgrenciler_Sinif_ID] ON [SinifOgrenciler];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DersProgramlari_Siniflar_Sinif_ID')
                    ALTER TABLE [DersProgramlari] DROP CONSTRAINT [FK_DersProgramlari_Siniflar_Sinif_ID];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DersProgramlari_Sinif_ID' AND object_id = OBJECT_ID('DersProgramlari'))
                    DROP INDEX [IX_DersProgramlari_Sinif_ID] ON [DersProgramlari];
            ");
        }
    }
}