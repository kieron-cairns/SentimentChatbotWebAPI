﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SentimentChatbotWebAPI.Data;

#nullable disable

namespace SentimentChatbotWebAPI.Migrations
{
    [DbContext(typeof(SentimentQueryHistoryContext))]
    [Migration("20230703102019_Initial-Test-DB-Create")]
    partial class InitialTestDBCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SentimentChatbotWebAPI.Models.QueryHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("QueryResult")
                        .IsRequired()
                        .HasColumnType("nvarchar(8)");

                    b.Property<string>("QueryText")
                        .IsRequired()
                        .HasColumnType("nvarchar(300)");

                    b.HasKey("Id");

                    b.ToTable("QueryHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
