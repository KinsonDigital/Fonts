using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SixLabors.Fonts.Tables.General;

namespace SixLabors.Fonts.Tables
{
    internal class TableLoader
    {
        public static TableLoader Default { get; } = new TableLoader();

        public TableLoader()
        {
            // we will hard code mapping registration in here for all the tables
            this.Register(NameTable.Load);
        }

        private Dictionary<string, Func<BinaryReader, Table>> loaders = new Dictionary<string, Func<BinaryReader, Table>>();
        private Dictionary<Type, string> types = new Dictionary<Type, string>();


        public string GetTag(Type type)
        {
            if (this.types.ContainsKey(type))
            {
                return this.types[type];
            }

            return null;
        }
        internal IEnumerable<string> TableTags(IEnumerable<Type> typeFilter)
        {
            if (typeFilter == null)
            {
                return this.types.Values;
            }

            return typeFilter.Select(x => this.types[x]).ToArray();
        }

        internal IEnumerable<Type> RegisterdTypes()
        {
            return this.types.Keys;
        }

        private void Register<T>(string tag, Func<BinaryReader, T> createFunc) where T : Table
        {
            lock (this.loaders)
            {
                if (!this.loaders.ContainsKey(tag))
                {
                    this.loaders.Add(tag, createFunc);
                    this.types.Add(typeof(T), tag);
                }
            }
        }

        private void Register<T>(Func<BinaryReader, T> createFunc) where T : Table
        {
            var name =
                typeof(T).GetTypeInfo()
                    .CustomAttributes
                    .First(x => x.AttributeType == typeof(TableNameAttribute))
                    .ConstructorArguments[0].Value.ToString();

            this.Register(name, createFunc);
        }

        internal Table Load(string Tag, BinaryReader reader)
        {
            // loader missing register an unknow type loader and carry on
            if (!this.loaders.ContainsKey(Tag))
            {
                return new UnknownTable(Tag);
            }

            return this.loaders[Tag]?.Invoke(reader);
        }

    }
}