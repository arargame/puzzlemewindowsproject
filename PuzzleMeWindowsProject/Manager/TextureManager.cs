﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleMeWindowsProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleMeWindowsProject.Manager
{
    public class TextureManager
    {
        public Texture2D Texture { get; set; }

        public TextureManager Load(Texture2D texture)
        {
            Texture = texture;

            return this;
        }

        public TextureManager Load(string textureName)
        {
            Texture = Global.Content.Load<Texture2D>(textureName);

            return this;
        }

        public Color[] GetColors()
        {
            return GetColors(Texture);
        }

        public static Color[] GetColors(Texture2D texture)
        {
            Color[] colors = new Color[texture.Width * texture.Height];

            texture.GetData(colors);

            return colors;
        }

        public static Texture2D CreateTexture2D(string assetName)
        {
            Texture2D newTexture = null;

            newTexture = Global.Content.Load<Texture2D>(assetName);

            return newTexture;
        }

        public static Texture2D CreateTexture2DBySingleColor(Color color,int width,int height)
        {
            Texture2D newTexture = null;

            try
            {
                Color[] colors = new Color[width * height];

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = color;
                }

                newTexture = new Texture2D(Global.GraphicsDevice, width, height);

                newTexture.SetData<Color>(colors);
            }
            catch (Exception ex)
            {   
                throw ex;
            }

            return newTexture;
        }


        public static Texture2D Crop(Texture2D originalTexture, Rectangle sourceRectangle)
        {
            Texture2D cropTexture = null;

            try
            {
                sourceRectangle.X = MathHelper.Clamp(sourceRectangle.X, 0, originalTexture.Width - sourceRectangle.Width);
                sourceRectangle.Y = MathHelper.Clamp(sourceRectangle.Y, 0, originalTexture.Height - sourceRectangle.Height);

                cropTexture = new Texture2D(Global.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);

                Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];

                originalTexture.GetData(0, sourceRectangle, data, 0, data.Length);

                cropTexture.SetData(data);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            return cropTexture;
        }

        public static List<Piece> Crop(Texture2D originalTexture, int rowCount, int columnCount)
        {
            var pieces = new List<Piece>();

            var size = new Vector2(originalTexture.Width, originalTexture.Height);

            var pieceSize = new Vector2(size.X / columnCount, size.Y / rowCount);

            pieces = Piece.To2DPieceArray(pieceSize, rowCount, columnCount)
                            .OfType<Piece>()
                            .ToList();

            var number = 0;

            foreach (var piece in pieces)
            {
                piece.SetTexture(Crop(originalTexture, new Rectangle((int)piece.Position.X, (int)piece.Position.Y, (int)piece.Size.X, (int)piece.Size.Y)));
                piece.ImageNumber = number++;
            }

            return pieces;
        }
    }
}