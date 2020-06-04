#region license
// Copyright (C) 2020 ClassicUO Development Community on Github
// 
// This project is an alternative client for the game Ultima Online.
// The goal of this is to develop a lightweight client considering
// new technologies.
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System;
using System.Runtime.CompilerServices;

using ClassicUO.Configuration;
using ClassicUO.IO.Resources;
using ClassicUO.Renderer;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using IDrawable = ClassicUO.Interfaces.IDrawable;

namespace ClassicUO.Game.GameObjects
{
    internal abstract partial class GameObject
    {
        protected static Vector3 HueVector;
        public Rectangle FrameInfo;

        protected bool IsFlipped;

        public bool UseObjectHandles { get; set; }

        public bool ClosedObjectHandles { get; set; }

        public bool ObjectHandlesOpened { get; set; }

        public byte AlphaHue { get; set; }

        public bool DrawTransparent { get; set; }

        public bool AllowedToDraw { get; set; } = true;


        private static readonly Lazy<DepthStencilState> _stencil = new Lazy<DepthStencilState>(() =>
        {
            DepthStencilState state = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.GreaterEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 0,
                //DepthBufferEnable = true,
                //DepthBufferWriteEnable = true,
            };


            return state;
        });


        public virtual bool Draw(UltimaBatcher2D batcher, int posX, int posY)
        {
            //if (DrawTransparent)
            //{
            //    int x = RealScreenPosition.X;
            //    int y = RealScreenPosition.Y;
            //    int fx = (int) (World.Player.RealScreenPosition.X + World.Player.Offset.X);
            //    int fy = (int) (World.Player.RealScreenPosition.Y + (World.Player.Offset.Y - World.Player.Offset.Z));

            //    int dist = Math.Max(Math.Abs(x - fx), Math.Abs(y - fy));
            //    int maxDist = ProfileManager.Current.CircleOfTransparencyRadius;

            //    if (dist <= maxDist)
            //    {
            //        HueVector.Z = MathHelper.Lerp(1f, 1f - dist / (float)maxDist, 0.5f);
            //        //HueVector.Z = 1f - (dist / (float)maxDist);
            //    }
            //    else
            //        HueVector.Z = 1f - AlphaHue / 255f;
            //}
            //else if (AlphaHue != 255)
            //    HueVector.Z = 1f - AlphaHue / 255f;

            //if (!batcher.DrawSprite(Texture, posX - Bounds.X, posY - Bounds.Y, IsFlipped, ref HueVector))
            //    return false;



            //if (DrawTransparent)
            //{
            //    int maxDist = ProfileManager.Current.CircleOfTransparencyRadius + 44;

            //    int fx = (int) (World.Player.RealScreenPosition.X + World.Player.Offset.X);
            //    int fy = (int) (World.Player.RealScreenPosition.Y + (World.Player.Offset.Y - World.Player.Offset.Z));

            //    fx -= posX;
            //    fy -= posY;
            //    int dist = (int) Math.Sqrt(fx * fx + fy * fy);

            //    //dist = Math.Max(Math.Abs(fx - x), Math.Abs(fy - y));

            //    if (dist <= maxDist)
            //    {
            //        switch (ProfileManager.Current.CircleOfTransparencyType)
            //        {
            //            default:
            //            case 0:
            //                HueVector.Z = 0.75f;
            //                break;
            //            case 1:
            //                HueVector.Z = MathHelper.Lerp(1f, 0f, (dist / (float) maxDist));
            //                break;
            //        }

            //        batcher.DrawSprite(Texture, posX - Bounds.X, posY - Bounds.Y, IsFlipped, ref HueVector);

            //        if (AlphaHue != 255)
            //            HueVector.Z = 1f - AlphaHue / 255f;
            //        else
            //            HueVector.Z = 0;

            //        batcher.SetStencil(_stencil.Value);
            //        batcher.DrawSprite(Texture, posX - Bounds.X, posY - Bounds.Y, IsFlipped, ref HueVector);
            //        batcher.SetStencil(null);
            //        goto COT;
            //    }
            //}

            //if (AlphaHue != 255)
            //    HueVector.Z = 1f - AlphaHue / 255f;

            //if (!batcher.DrawSprite(Texture, posX - Bounds.X, posY - Bounds.Y, IsFlipped, ref HueVector))
            //    return false;

            //COT:

            //Select(posX, posY);

            //Texture.Ticks = Time.Ticks;

            return true;
        }

        [MethodImpl(256)]
        protected static void ResetHueVector()
        {
            HueVector.X = 0;
            HueVector.Y = 0;
            HueVector.Z = 0;
        }

        public Rectangle GetOnScreenRectangle()
        {
            Rectangle prect = Rectangle.Empty;

            prect.X = (int) (RealScreenPosition.X - FrameInfo.X + 22 + Offset.X);
            prect.Y = (int) (RealScreenPosition.Y - FrameInfo.Y + 22 + (Offset.Y - Offset.Z));
            prect.Width = FrameInfo.Width;
            prect.Height = FrameInfo.Height;

            return prect;
        }

        public virtual bool TransparentTest(int z)
        {
            return false;
        }

        [MethodImpl(256)]
        public bool ProcessAlpha(int max)
        {
            if (ProfileManager.Current != null && !ProfileManager.Current.UseObjectsFading)
            {
                AlphaHue = (byte) max;

                return max != 0;
            }

            bool result = false;

            int alpha = AlphaHue;

            if (alpha > max)
            {
                alpha -= 25;

                if (alpha < max)
                    alpha = max;

                result = true;
            }
            else if (alpha < max)
            {
                alpha += 25;

                if (alpha > max)
                    alpha = max;

                result = true;
            }

            AlphaHue = (byte) alpha;

            return result;
        }


        protected static void DrawLand(UltimaBatcher2D batcher, ushort graphic, int x, int y, ref Vector3 hue)
        {
            var texture = ArtLoader.Instance.GetLandTexture(graphic);
            if (texture != null)
            {
                texture.Ticks = Time.Ticks;

                batcher.DrawSprite(texture, x, y, false, ref hue);
            }
        }

        protected static void DrawLand(
            UltimaBatcher2D batcher,
            ushort graphic, int x, int y,
            ref Rectangle rectangle,
            ref Vector3 n0, ref Vector3 n1, ref Vector3 n2, ref Vector3 n3,
            ref Vector3 hue)
        {
            var texture = TexmapsLoader.Instance.GetTexture(graphic);
            if (texture != null)
            {
                texture.Ticks = Time.Ticks;

                batcher.DrawSpriteLand(texture, x, y, ref rectangle, ref n0, ref n1, ref n2, ref n3, ref hue);
            }
        }

        protected static void DrawStatic(UltimaBatcher2D batcher, ushort graphic, int x, int y, ref Vector3 hue)
        {
            var texture = ArtLoader.Instance.GetTexture(graphic);
            if (texture != null)
            {
                texture.Ticks = Time.Ticks;
                ref var index = ref ArtLoader.Instance.GetValidRefEntry(graphic + 0x4000);

                batcher.DrawSprite(texture, x - index.Width, y - index.Height, false, ref hue);
            }
        }

        protected static void DrawGump(UltimaBatcher2D batcher, ushort graphic, int x, int y, ref Vector3 hue)
        {
            var texture = GumpsLoader.Instance.GetTexture(graphic);
            if (texture != null)
            {
                texture.Ticks = Time.Ticks;

                batcher.DrawSprite(texture, x, y, false, ref hue);
            }
        }

        protected static void DrawStaticRotated(UltimaBatcher2D batcher, ushort graphic, int x, int y, int destX, int destY, float angle, ref Vector3 hue)
        {
            var texture = ArtLoader.Instance.GetTexture(graphic);
            if (texture != null)
            {
                texture.Ticks = Time.Ticks;

                batcher.DrawSpriteRotated(texture, x, y, destX, destY, ref hue, angle);
            }
        }

        protected static void DrawStaticAnimated(UltimaBatcher2D batcher, ushort graphic, int x, int y, ref Vector3 hue)
        {
            ref var index = ref ArtLoader.Instance.GetValidRefEntry(graphic + 0x4000);

            graphic = (ushort) (graphic + index.AnimOffset);

            var texture = ArtLoader.Instance.GetTexture(graphic);
            if (texture != null)
            {
                texture.Ticks = Time.Ticks;
                index = ref ArtLoader.Instance.GetValidRefEntry(graphic + 0x4000);

                batcher.DrawSprite(texture, x - index.Width, y - index.Height, false, ref hue);
            }
        }

        protected static readonly Lazy<DepthStencilState> StaticTransparentStencil = new Lazy<DepthStencilState>(() =>
        {
            DepthStencilState state = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.GreaterEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 0,
                //DepthBufferEnable = true,
                //DepthBufferWriteEnable = true,
            };


            return state;
        });
    }
}