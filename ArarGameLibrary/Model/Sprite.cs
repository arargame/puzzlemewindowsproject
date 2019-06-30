﻿using ArarGameLibrary.Effect;
using ArarGameLibrary.Extension;
using ArarGameLibrary.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArarGameLibrary.Model
{
    public interface IBaseObject
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        DateTime CreateDate { get; set; }

        DateTime UpdateDate { get; set; }

        bool IsInPerformanceMode { get; set; }
    }

    public interface IXna
    {
        void Initialize();

        void LoadContent(Texture2D texture = null);

        void UnloadContent();

        void Update(GameTime gameTime = null);

        void Draw(SpriteBatch spriteBatch = null);
    }

    public interface IDrawableObject : IXna
    {
        Rectangle CollisionRectangle { get; set; }
        Color Color { get; set; }

        Rectangle DestinationRectangle { get; }

        bool IsActive { get; set; }
        bool IsAlive { get; set; }
        bool IsVisible { get; set; }

        float LayerDepth { get; set; }

        Vector2 Origin { get; set; }

        Vector2 Position { get; set; }

        float Rotation { get; set; }

        float Scale { get; set; }
        Vector2 Size { get; set; }
        Rectangle SourceRectangle { get; set; }
        Vector2 Speed { get; set; }
        SpriteEffects SpriteEffects { get; set; }
        SpriteBatch SpriteBatch { get; set; }

        Texture2D Texture { get; set; }
    }

    public interface IClickableIbject
    {
        bool IsClickable { get; set; }
        bool IsHovering { get; set; }
        bool IsSelecting { get; set; }
        void SetClickable(bool enable);
    }

    public abstract class BaseObject : IBaseObject, ICloneable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsInPerformanceMode { get; set; }

        public BaseObject()
        {
            Id = Guid.NewGuid();

            CreateDate = UpdateDate = DateTime.Now;
        }

        public Object Clone()
        {
            return MemberwiseClone();
        }
    }


    public abstract class Sprite : BaseObject, IDrawableObject, IClickableIbject
    {
        #region Properties

        public Rectangle CollisionRectangle { get; set; }
        public Color Color { get; set; }
        public ClampManager ClampManager { get; set; }

        public Rectangle DestinationRectangle
        {
            get
            {
                return new Rectangle((int)Math.Ceiling(Position.X), (int)Math.Ceiling(Position.Y), (int)Math.Ceiling(Size.X * Scale), (int)Math.Ceiling((Size.Y * Scale)));
            }
        }
        public int DrawMethodType { get; set; }

        public bool IsActive { get; set; }
        public bool IsAlive { get; set; }
        public bool IsClickable { get; set; }
        public bool IsPulsating { get; set; }
        public bool IsHovering { get; set; }
        public bool IsSelecting { get; set; }
        public bool IsVisible { get; set; }

        //From 0f to 1f where 1f is the top layer
        public float LayerDepth { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public float Scale { get; set; }
        public bool SimpleShadowVisibility { get; set; }
        public Vector2 Size { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Vector2 Speed { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        public TestInfo TestInfo { get; set; }
        public Texture2D Texture { get; set; }

        public delegate void ChangingSomething();
        public event ChangingSomething OnChangeRectangle;

        public List<EffectManager> Effects = new List<EffectManager>();

        #endregion

        #region Constructor 

        public Sprite()
        {
            Initialize();
        }

        #endregion

        #region Functions

        public virtual void Initialize()
        {
            OnChangeRectangle += SetRectangle;

            IsActive = IsAlive = IsVisible = true;

            SetStartingSettings();

            //DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            //destinationRectangle = new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y), (int)size.X, (int)size.Y);

            SourceRectangle = new Rectangle(0, 0, (int)Size.X, (int)Size.Y);

            Color = Color.White;

            Effects.Add(new PulsateEffect(this));
            Effects.Add(new SimpleShadowEffect(this,new Vector2(5,5)));

            ClampManager = new ClampManager(this);

            TestInfo = new TestInfo(this);

            SpriteBatch = Global.SpriteBatch;
        }

        public virtual void LoadContent(Texture2D texture = null)
        {
            SetTexture(texture);
        }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime = null)
        {
            if (IsActive)
            {
                SetRectangle();

                SetOrigin();

                //if (IsPulsating)
                //{
                //    Scale = General.Pulsate();
                //}

                foreach (var effect in Effects)
                {
                    effect.Update();
                }

                ClampManager.Update();

                TestInfo.Update();

                if (IsClickable)
                {
                    IsHovering = InputManager.IsHovering(DestinationRectangle);
                    IsSelecting = InputManager.Selected(DestinationRectangle);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch = null)
        {
            SetSpriteBatch(spriteBatch);

            if (IsActive && IsVisible)
            {
                if (Texture != null)
                {
                    switch (DrawMethodType)
                    {
                        case 1:
                            SpriteBatch.Draw(Texture, DestinationRectangle, Color);
                            break;

                        case 2:
                            SpriteBatch.Draw(Texture, Position, Color);
                            break;

                        case 3:
                            SpriteBatch.Draw(Texture, DestinationRectangle, SourceRectangle, Color);
                            break;

                        case 4:
                            SpriteBatch.Draw(Texture, Position, SourceRectangle, Color);
                            break;

                        case 5:
                            SpriteBatch.Draw(Texture, DestinationRectangle, SourceRectangle, Color, Rotation, Origin, SpriteEffects, LayerDepth);
                            break;

                        case 6:
                            SpriteBatch.Draw(Texture, Position, SourceRectangle, Color, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
                            break;

                        case 7:
                            SpriteBatch.Draw(Texture, Position, SourceRectangle, Color, Rotation, Origin, new Vector2(Scale), SpriteEffects, LayerDepth);
                            break;

                        default:
                            SpriteBatch.Draw(Texture, DestinationRectangle, Color);
                            break;
                    }
                }

                foreach (var effect in Effects)
                {
                    effect.Draw();
                }

                //  TestInfo.Draw();
            }
        }

        public void Align(Vector2 offset, Rectangle? parentRect = null)
        {
            if (parentRect == null)
                parentRect = Global.ViewportRect;

            SetPosition(new Vector2((int)(parentRect.Value.Position().X + offset.X), (int)(parentRect.Value.Position().Y + offset.Y)));
        }

        public T GetEffect<T>() where T : EffectManager
        {
            return EffectManager.Get<T>(Effects);
        }

        public void Pulsate(bool enable)
        {
            IsPulsating = enable;

            var pulsateEffect = Effects.FirstOrDefault(e => e is PulsateEffect);

            if (pulsateEffect == null)
                return;

            if (IsPulsating)
                pulsateEffect.Start();
            else
                pulsateEffect.End();
        }

        public void ShowSimpleShadow(bool enable)
        {
            SimpleShadowVisibility = enable;

            var simpleShadowEffect = GetEffect<SimpleShadowEffect>();

            if (simpleShadowEffect == null)
                return;

            if (SimpleShadowVisibility)
                simpleShadowEffect.Start();
            else
                simpleShadowEffect.End();
        }

        #region SetFunctions

        public void SetActive(bool enable)
        {
            IsActive = enable;
        }

        public void SetAlive(bool enable)
        {
            IsAlive = enable;
        }

        public void SetClickable(bool enable)
        {
            IsClickable = enable;
        }

        public void SetColor(Color color)
        {
            Color = color;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetDrawMethodType(int methodType)
        {
            DrawMethodType = methodType;
        }

        public void SetLayerDepth(float layerDepth)
        {
            LayerDepth = layerDepth;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        private void SetOrigin(Vector2? origin = null)
        {
            if (origin != null)
                Origin = origin.Value;
            else
                Origin = Vector2.Zero;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;

            OnChangeRectangle();
        }

        public void SetRectangle()
        {
            SourceRectangle = new Rectangle(0, 0, Texture != null ? Texture.Width : DestinationRectangle.Width, Texture != null ? Texture.Height : DestinationRectangle.Height);
            //CollisionRectangle = new Rectangle(Des);
            //SourceRectangle = new Rectangle(animation.FrameBounds.X, animation.FrameBounds.Y, (int)Size.X, (int)Size.Y);
            //            destinationRectangle = new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y), (int)size.X, (int)size.Y);
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void SetScale(float scale)
        {
            Scale = scale;

            OnChangeRectangle();
        }

        public void SetSize(Vector2 size)
        {
            Size = size;

            OnChangeRectangle();
        }

        public void SetSpeed(Vector2 speed)
        {
            Speed = speed;
        }

        public void SetSpriteBatch(GraphicsDevice graphicsDevice = null)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice ?? Global.GraphicsDevice);
        }

        public void SetSpriteBatch(SpriteBatch spriteBatch = null)
        {
            if(spriteBatch!=null)
                SpriteBatch = spriteBatch;
        }

        public void SetSpriteEffects(SpriteEffects effects)
        {
            SpriteEffects = effects;
        }

        public void SetTexture(string name)
        {
            Texture = Global.Content.Load<Texture2D>(name);
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
        }

        public void SetVisible(bool enable)
        {
            IsVisible = enable;
        }

        #endregion


        #region StartingFunctions

        private void SetStartingSettings()
        {
            SetClickable(false);

            SetStartingLayerDepth();

            SetOrigin();

            SetStartingPosition();

            SetStartingRotation();

            SetStartingScale();
            SetStartingSize();
            SetStartingSpeed();
            SetStartingSpriteEffects();
        }

        public virtual void SetStartingLayerDepth()
        {
            SetLayerDepth(0.5f);
        }

        public virtual void SetStartingScale()
        {
            SetScale(1f);
        }

        public virtual void SetStartingSpriteEffects()
        {
            SpriteEffects = SpriteEffects.None;
        }

        public virtual void SetStartingPosition()
        {
            SetPosition(Vector2.Zero);
        }

        public virtual void SetStartingRotation()
        {
            SetRotation(0f);
        }

        public virtual void SetStartingSize()
        {
            SetSize(Vector2.Zero);
        }

        public virtual void SetStartingSpeed()
        {
            SetSpeed(Vector2.Zero);
        }

        #endregion


        #endregion



    }
}
