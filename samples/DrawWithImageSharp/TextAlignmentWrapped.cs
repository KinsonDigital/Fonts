// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes.Temp;

namespace DrawWithImageSharp
{
    public static class TextAlignmentWrapped
    {
        public static void Generate(Font font)
        {
            int wrappingWidth = 400;
            int size = (wrappingWidth + (wrappingWidth / 3)) * 3;
            using (var img = new Image<Rgba32>(size, size))
            {
                img.Mutate(x => x.Fill(Color.White));

                foreach (VerticalAlignment v in Enum.GetValues(typeof(VerticalAlignment)))
                {
                    foreach (HorizontalAlignment h in Enum.GetValues(typeof(HorizontalAlignment)))
                    {
                        Draw(img, font, v, h, wrappingWidth);
                    }
                }

                img.Save("Output/AlignmentWrapped.png");
            }
        }

        public static void Draw(Image<Rgba32> img, Font font, VerticalAlignment vert, HorizontalAlignment horiz, float wrappingWidth)
        {
            Vector2 location = Vector2.Zero;
            switch (vert)
            {
                case VerticalAlignment.Top:
                    location.Y = 0;
                    break;
                case VerticalAlignment.Center:
                    location.Y = img.Height / 2F;
                    break;
                case VerticalAlignment.Bottom:
                    location.Y = img.Height;
                    break;
                default:
                    break;
            }

            switch (horiz)
            {
                case HorizontalAlignment.Left:

                    location.X = 0;
                    break;
                case HorizontalAlignment.Right:
                    location.X = img.Width;
                    break;
                case HorizontalAlignment.Center:
                    location.X = img.Width / 2F;
                    break;
                default:
                    break;
            }

            var glyphBuilder = new GlyphBuilder();

            var renderer = new TextRenderer(glyphBuilder);

            var style = new RendererOptions(font, 72, location)
            {
                ApplyKerning = true,
                TabWidth = 4,
                WrappingWidth = wrappingWidth,
                HorizontalAlignment = horiz,
                VerticalAlignment = vert
            };

            string text = $"    {horiz}     {vert}         {horiz}     {vert}         {horiz}     {vert}     ";
            renderer.RenderText(text, style);

            IEnumerable<IPath> shapesToDraw = glyphBuilder.Paths;
            img.Mutate(x => x.Fill(Color.Black, glyphBuilder.Paths));

            Rgba32 f = Color.Fuchsia;
            f.A = 128;
            img.Mutate(x => x.Fill(Color.Black, glyphBuilder.Paths));
            img.Mutate(x => x.Draw(f, 1, glyphBuilder.Boxes));

            img.Mutate(x => x.Draw(Color.Lime, 1, glyphBuilder.TextBox));
        }
    }
}
