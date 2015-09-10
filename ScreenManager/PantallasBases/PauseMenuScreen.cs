using ScreenManager.MenuScren;

namespace ScreenManager.PantallasBases
{
    /// <summary>
    /// Una pantalla de pause llamada durante el desarrollo del juego.
    /// Las opciones son:
    /// <list type="bullet">
    /// <item> 
    /// <description>Volver al Juego.</description> 
    /// </item> 
    /// <item> 
    /// <description>Salir del Juego.</description> 
    /// </item>
    /// </list> 
    /// </summary>    
    public class PauseMenuScreen : BaseMenuScreen
    {
        #region Constructor

        /// <summary>
        /// Constructor del menu, asigna las opciones y los eventos a la Screen
        /// </summary>
        public PauseMenuScreen()
            : base("Pausa")
        {
            // Crea las entries
            MenuEntry volverGameMenuEntry = new MenuEntry(this, "Volver al Juego");
            MenuEntry salirGameMenuEntry = new MenuEntry(this, "Salir del juego");

            // Añade los manejadores de evento
            volverGameMenuEntry.Selected += OnCancel;
            salirGameMenuEntry.Selected += SalirGameMenuEntrySelected;

            // Pone las entries en el menú
            MenuEntries.Add(volverGameMenuEntry);
            MenuEntries.Add(salirGameMenuEntry);
        }


        #endregion

        #region Events


        /// <summary>
        /// Manejador de eventos para cuando se desee salir del juego
        /// </summary>
        /// <param name="sender">Objeto del evento</param>
        /// <param name="e">Player que gatillo el evento</param>
        void SalirGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Estas seguro?";

            PopupScreen confirmQuitMessageBox = new PopupScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManagerController.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Manejador de eventos para cuando el usuario decide salir del juego.
        /// </summary>
        /// <param name="sender">Objeto del evento</param>
        /// <param name="e">Player que gatillo el evento</param>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManagerController.Game.Exit();
        }


        #endregion
    }
}
