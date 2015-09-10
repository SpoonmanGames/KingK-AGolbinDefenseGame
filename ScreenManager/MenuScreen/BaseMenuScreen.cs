using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.StateControl;
using System;
using System.Collections.Generic;

namespace ScreenManager.MenuScren
{
    /// <summary>
    /// Clase base para las Screen que tendrán menus de diferentes tipos.
    /// El usuario se puede mover arriba o abajo / izquierda o derecha. Puede
    /// cancelar para volver a la pantalla anterior y seleccionar un entry (MenuEntry)
    /// </summary>
    public abstract class BaseMenuScreen : GameScreen
    {
        #region Fields

        /// <summary>
        /// Titulo del menú.
        /// </summary>
        string menuTitle;


        /// <summary>
        /// Lista que contiene los MenuEntry en orden
        /// </summary>
        public int MenuEntriesCount { get { return this.menuEntries.Count; } }
        private List<MenuEntry> menuEntries = new List<MenuEntry>();

        /// <summary>
        /// Valor que de fine la posición inicial Y para los entries.
        /// Por defecto: 175f
        /// </summary>
        public float YStartingEntryPosition { get { return this.yStartingEntryPosition; } set { this.yStartingEntryPosition = value; } }
        private float yStartingEntryPosition;

        /// <summary>
        /// Valor que define la separación en Y para los entries.
        /// Por defecto es 0
        /// </summary>
        public int YOffSetPosition { get { return this.yOffSetPosition; } set { this.yOffSetPosition = value; } }
        private int yOffSetPosition;

        /// <summary>
        /// Valor que de fine la posición inicial Y para el titulo.
        /// Por defecto es 80
        /// </summary>
        public int YTitlePosition { get { return this.yTitlePosition; } set { this.yTitlePosition = value; } }
        private int yTitlePosition;

        #endregion

        #region Properties

        /// <summary>
        /// Para identificar cual MenuEntry está siendo seleccionado en este momento
        /// </summary>
        public int SelectedEntry { get { return this.selectedEntry; } set { this.selectedEntry = value; } }
        private int selectedEntry = 0;

        /// <summary>
        /// Obtiene la lista de menuEntries para poder agregar o quitar entries
        /// según sea necesario
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        #endregion

        #region Constructor


        /// <summary>
        /// Constructor que asigna los tiempos de transición y el titulo del menú.
        /// </summary>
        /// <param name="menuTitle">Titulo del Menú</param>
        public BaseMenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.yStartingEntryPosition = 175f;
            this.yOffSetPosition = 0;
            this.yTitlePosition = 80;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Reacciona al input del usuario, cambiando entre Entries y aceptando
        /// o cancelando el menú.
        /// </summary>
        /// <param name="input">Input de los controles</param>
        public override void HandleInput(InputState input)
        {
            //Se mueve al entry superior (o al último)
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            //Se mueve al siguiente entry o al primero
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            /* Acepta o cancela el menú. playerIndex guarda quien es el player que ha hecho la acción.
             * De esta forma es posible saber quien gatillo el evento.
             */
            PlayerIndex playerIndex;

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Maneja cuando el usuario ha seleccionado un MenuEntry
        /// </summary>
        /// <param name="entryIndex">Index del MenuEntry seleccinado</param>
        /// <param name="playerIndex">Index del jugador que ha realizado la acción</param>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if(menuEntries.Count > 0)
                menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Maneja cuando el usuario ha seleccionado de forma inversa un MenuEntry
        /// </summary>
        /// <param name="entryIndex">Index del MenuEntry seleccinado</param>
        /// <param name="playerIndex">Index del jugador que ha realizado la acción</param>
        protected virtual void OnReverseSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if (menuEntries.Count > 0)
                menuEntries[entryIndex].OnReverseSelectEntry(playerIndex);
        }

        /// <summary>
        /// Maneja cuando el usuario ha seleccionado de forma inversa un MenuEntry
        /// </summary>
        /// <param name="entryIndex">Index del MenuEntry seleccinado</param>
        /// <param name="playerIndex">Index del jugador que ha realizado la acción</param>
        protected virtual void OnForwardSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if (menuEntries.Count > 0)
                menuEntries[entryIndex].OnForwardSelectEntry(playerIndex);
        }

        /// <summary>
        /// Maneja cuando el usuario a cancelado el menú.
        /// </summary>
        /// <param name="playerIndex">Index del jugador que ha realizado la acción</param>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Permite que sea más facil usar OnCancel como un Event de MenuEntry
        /// </summary>
        /// <param name="sender"> Object </param>
        /// <param name="e">Evento generado que contiene el indice del player que lo gatilló</param>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Permite que la pantalla mantenga actualizada la posición de los MenuEntries.
        /// Por defecto, todos los MenuEntries son ordenados verticalmente y centrados en la Screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            /* Hace que el menu se deslice hacia su posición durante la transición, usando
             * una función cuadratica se logra que el movimiento sea más lento
             * al llegar al final
             */
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Posición donde comenzarán a aparecer los entries, Y es fijo, X cambia para centrarse
            Vector2 position = new Vector2(0f, this.yStartingEntryPosition);

            //Se actualiza cada entry en orden
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // Se obtiene la posición del centro para el Entry dado
                position.X = ScreenManagerController.GraphicsDevice.Viewport.Width / 2 - menuEntry.TextWidth / 2;

                //Si está en TransitionOn 
                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                //Asigna la nueva posición al Entry
                menuEntry.Position = position;

                //Pasa al siguiente entry, actualizando su posición Y
                position.Y += menuEntry.TextHeight + yOffSetPosition;
            }
        }


        /// <summary>
        /// Update para el menú completo
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        /// <param name="otherScreenHasFocus">Para saber si esta Screen tiene el foco o no</param>
        /// <param name="coveredByOtherScreen">Para saber si está siendo tapada por otra Screen o no</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Llama al Update de su padre
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Actualiza cada menuEntry
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(isSelected, gameTime);
            }
        }


        /// <summary>
        /// Dibuja el Screen
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public override void Draw(GameTime gameTime)
        {
            //Se asegura que las entries estén en el lugar indicado
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManagerController.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;
            SpriteFont font = ScreenManagerController.Font;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Dibuja cada Entry en orden
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(isSelected, gameTime);
            }


            /** TÍTULO DEL MENÚ **/

            /* Hace que el título del menú se deslice hacia su posición durante la transición, usando
             * una función cuadratica se logra que el movimiento sea más lento
             * al llegar al final
             */
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //Dibuja el titulo centrado en la Screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, yTitlePosition);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        #endregion

    }
}
