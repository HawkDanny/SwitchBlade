﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SwagSword
{
    class StatButton
    {
        //Fields
        private Game1 mainMan;
        private Texture2D texture;
        private Rectangle rect;
        private bool mousedOver;
        private Player player;
        private string type;
        private bool enabled;

        //Properties
        public Texture2D Texture { get { return texture; } set { texture = value; } }
        public Rectangle Rect { get { return rect; } set { rect = value; } }

        /// <summary>
        /// Creates a button used for increasing a player's stat multipliers
        /// </summary>
        /// <param name="texture">Button Texture</param>
        /// <param name="rect">Position and Size of the button</param>
        /// <param name="player">Player whos stats will be multiplied</param>
        /// <param name="type">the type of button</param>
        public StatButton(Game1 mainMan, Rectangle rect, Player player, string type)
        {
            this.mainMan = mainMan;
            texture = mainMan.DrawMan.LevelUpButtonTexture;
            this.rect = rect;
            this.player = player;
            mousedOver = false;
            enabled = false;
            this.type = type;
        }

        public void Update()
        {
            isMousedOver();
            if (mousedOver)
            {
                isClicked();
            }
            if(mainMan.GameMan.Players[0].SkillPoints > 0)
            {
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }

        public void isMousedOver()
        {
            if (rect.Contains(mainMan.InputMan.PointerPosition.X, mainMan.InputMan.PointerPosition.Y))
            {
                mousedOver = true;
            }
            else
            {
                mousedOver = false;
            }
        }

        public virtual void isClicked()
        {
            if (mainMan.InputMan.PrevMState.LeftButton == ButtonState.Released && mainMan.InputMan.MState.LeftButton == ButtonState.Pressed && enabled)
            {
                if (type == "health")
                {
                    player.HealthMultiplier += .1f;
                    player.Character.MaxHealth = (int)(player.HealthMultiplier * player.Character.MaxHealth);
                }
                if (type == "damage")
                {
                    player.DamageMultiplier += .1f;
                    player.Character.Damage = (int)(player.DamageMultiplier * player.Character.Damage);
                }
                if (type == "knockback")
                {
                    player.KnockbackMultplier += .05f;
                    player.Character.Strength = (int)(player.KnockbackMultplier * player.Character.Strength);
                }
                if (type == "attackspeed")
                {
                    player.AttackSpeedMultiplier += .1f;
                    player.Character.AttackSpeedMin *= player.AttackSpeedMultiplier;
                    player.Character.AttackSpeedMax *= player.AttackSpeedMultiplier;
                }
                if (type == "movementspeed")
                {
                    player.MovementSpeedMultiplier += .1f;
                    player.Character.MovementSpeed *= player.MovementSpeedMultiplier;
                }
                player.SkillPoints--;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(enabled)
            {
                if (mousedOver)
                {
                    spriteBatch.Draw(texture, rect, Color.Blue);
                }
                else
                {
                    spriteBatch.Draw(texture, rect, Color.White);
                }

            }
            
        }
    }
}
