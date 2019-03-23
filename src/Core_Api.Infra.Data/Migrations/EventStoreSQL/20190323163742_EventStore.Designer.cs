using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Core_Api.Infra.Data.Context;

namespace Core_Api.Infra.Data.Migrations.EventStoreSQL
{
    [DbContext(typeof(EventStoreSQLContext))]
    [Migration("20190323163742_EventStore")]
    partial class EventStore
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Core_Api.Domain.Core.Events.StoredEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AggregateId");

                    b.Property<string>("Data");

                    b.Property<string>("MessageType")
                        .HasColumnName("Action")
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnName("CreationDate");

                    b.Property<string>("User");

                    b.HasKey("Id");

                    b.ToTable("StoredEvent");
                });
        }
    }
}
