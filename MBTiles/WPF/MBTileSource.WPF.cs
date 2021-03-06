﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapControl
{
    public class MBTileSource : TileSource, IDisposable
    {
        private readonly SQLiteConnection connection;

        public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        public MBTileSource(string file)
        {
            connection = new SQLiteConnection("Data Source=" + file + ";Version=3;");
            connection.Open();

            using (var command = new SQLiteCommand("select * from metadata", connection))
            {
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Metadata[(string)reader["name"]] = (string)reader["value"];
                }
            }
        }

        public override async Task<ImageSource> LoadImageAsync(int x, int y, int zoomLevel)
        {
            ImageSource imageSource = null;

            try
            {
                using (var command = new SQLiteCommand("select tile_data from tiles where zoom_level=@z and tile_column=@x and tile_row=@y", connection))
                {
                    command.Parameters.AddWithValue("@z", zoomLevel);
                    command.Parameters.AddWithValue("@x", x);
                    command.Parameters.AddWithValue("@y", (1 << zoomLevel) - y - 1);

                    var buffer = await command.ExecuteScalarAsync() as byte[];

                    if (buffer != null)
                    {
                        imageSource = await Task.Run(() =>
                        {
                            using (var stream = new MemoryStream(buffer))
                            {
                                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MBTileSource: {0}/{1}/{2}: {3}", zoomLevel, x, y, ex.Message);
            }

            return imageSource;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && connection != null)
            {
                connection.Close();
            }
        }
    }
}
