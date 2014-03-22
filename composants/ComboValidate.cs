using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Stl.Tme.Components.Controls
{
    /// <summary>
    /// <see cref="ComboBox"/> avec prise en charge des caratères entrée et escape.
    /// </summary>
    /// <remarks>
    /// Gestion de la validation et de l'annulation du texte entré.
    /// </remarks>    
    public partial class ComboValidate : ComboBox
    {
        private string lastText;

        /// <summary>
        /// Obtient ou détermine si le caractère de validation est pris en charge.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Obtient ou détermine si le caractère de validation est pris en charge.")]
        public bool TextValidate { get; set; }

        /// <summary>
        /// Obtient ou détermine si le caractère d'annulation est pris en charge.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Obtient ou détermine si le caractère d'annulation est pris en charge.")]
        public bool TextCancel { get; set; }

        /// <summary>
        /// Déclenché quand le caractère de validation a été tapé
        /// </summary>
        public event EventHandler<TextValidatingEventArgs> TextValidating;

        /// <summary>
        /// Déclenché quand le caractère d'annulation a été tapé
        /// </summary>
        public event EventHandler<CancelEventArgs> TextCancelling;

        /// <summary>
        /// Déclenché la validation s'est terminée
        /// </summary>
        public event EventHandler<EventArgs> TextValidated;

        /// <summary>
        /// Déclenché quand l'annulation s'est terminée
        /// </summary>
        public event EventHandler<EventArgs> TextCancelled;

        /// <summary>
        /// Constructeur
        /// </summary>
        public ComboValidate()
        {            
            InitializeComponent();
        }

        /// <summary>
        /// Déclenchement du processus de validation
        /// </summary>
        public void TextEnter()
        {               
            if (!TextValidate || DropDownStyle == ComboBoxStyle.DropDownList)
                return;
            Validate();            
        }

        /// <summary>
        /// Diffusion de la validation
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement associé à la diffusion de la validation
        /// </param>
        protected virtual void OnTextValidating(TextValidatingEventArgs e)
        {
            if (TextValidating != null)
                TextValidating(this, e);
        }

        /// <summary>
        /// Diffusion de la confirmation de la validation
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement associé à la confirmation de validation
        /// </param>
        protected virtual void OnTextValidated(EventArgs e)
        {
            if (TextValidating != null)
                TextValidated(this, e);
        }

        /// <summary>
        /// Diffusion de l'annulation du texte.
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement associé à l'annulation du texte entré.
        /// </param>
        protected virtual void OnTextCancelling(CancelEventArgs e)
        {
            if (TextValidating != null)
                TextCancelling(this, e);
        }

        /// <summary>
        /// Diffusion de la confirmation d'annulation du texte.
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement associé à la confirmation d'annulation du texte entré.
        /// </param>
        protected virtual void OnTextCancelled(EventArgs e)
        {
            if (TextValidating != null)
                TextCancelled(this, e);
        }        

        /// <summary>
        /// Diffusion de la frappe d'une touche avec traitement préalable si le caractère est pris en compte.
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement associé à la frappe clavier.
        /// </param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (TextValidate) 
                ProcessKeyPress(e);
            base.OnKeyPress(e);
        }

        #region Méthodes de service

        private void ProcessKeyPress(KeyPressEventArgs e)
        {     
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    Validate();
                    break;

                case (char)Keys.Escape:
                    Cancel();
                    break;
            }
        }

        /// <summary>
        /// Annulation du texte saisi
        /// </summary>
        private void Cancel()
        {
            CancelEventArgs ev = new CancelEventArgs();
            OnTextCancelling(ev);
            if (ev.Cancel)
                return;
            Text = lastText;
            OnTextCancelled(new EventArgs());           
        }

        /// <summary>
        /// Validation du texte saisi
        /// </summary>
        private void Validate()
        {
            TextValidatingEventArgs ev = new TextValidatingEventArgs() { Text = this.Text };
            OnTextValidating(ev);
            if (ev.Cancel)
                return;
            Text = ev.Text;
            lastText = ev.Text;
            OnTextValidated(new EventArgs());
        }

        #endregion
    }
    
    /// <summary>
    /// Descripteur d'événement décrivant la validation d'un texte.
    /// </summary>
    public class TextValidatingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Obtient ou détermine le texte à valider.
        /// </summary>
        public string Text { get; set; }
    }
}
