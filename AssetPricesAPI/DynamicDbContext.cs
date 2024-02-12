﻿using AssetPricesAPI.Metadata;
using Microsoft.EntityFrameworkCore;

namespace AssetPricesAPI
{
    public class DynamicDbContext: DbContext
    {
        string _version;

        public DynamicDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public List<MetadataEntity> _metaDataEntityList = [];

        public void AddMetadata(MetadataEntity metadataEntity) => _metaDataEntityList.Add(metadataEntity);

        public MetadataEntity GetMetadaEntity(Type type) => _metaDataEntityList.FirstOrDefault(p => p.EntityType == type);


        public void SetContextVersion(string version) => _version = version;

        public string GetContextVersion() => _version;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            foreach (var metadataEntity in _metaDataEntityList)
            {
                modelBuilder.Entity(metadataEntity.EntityType).ToTable(metadataEntity.TableName, metadataEntity.SchemaName).HasKey("Id");

                foreach (var metaDataEntityProp in metadataEntity.Properties)
                {
                    if (!metaDataEntityProp.IsNavigation)
                    {
                        var propBuilder = modelBuilder.Entity(metadataEntity.EntityType).Property(metaDataEntityProp.Name);

                        if (!string.IsNullOrEmpty(metaDataEntityProp.ColumnName))
                            propBuilder.HasColumnName(metaDataEntityProp.ColumnName);
                    }
                }
            }
            base.OnModelCreating(modelBuilder);
        }


    }
}
