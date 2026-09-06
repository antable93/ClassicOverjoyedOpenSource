using Wisej.Web.Markup;

namespace OverjoyedReleaseLocal
{
    partial class GuidedTour
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonsPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // HtmlText
            // 
            this.HtmlText.ScrollBars = Wisej.Web.ScrollBars.None;
            this.HtmlText.Size = new System.Drawing.Size(481, 183);
            this.HtmlText.Appear += new System.EventHandler(this.HtmlText_Appear);
            this.HtmlText.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            // 
            // TitleLabel
            // 
            this.TitleLabel.Anchor = Wisej.Web.AnchorStyles.None;
            this.TitleLabel.Dock = Wisej.Web.DockStyle.None;
            this.TitleLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.TitleLabel.Location = new System.Drawing.Point(0, -6);
            this.TitleLabel.Size = new System.Drawing.Size(296, 41);
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(302, 0);
            this.CloseButton.Movable = false;
            this.CloseButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.ButtonsPanel.AutoSize = false;
            this.ButtonsPanel.Dock = Wisej.Web.DockStyle.None;
            this.ButtonsPanel.Location = new System.Drawing.Point(352, 11);
            this.ButtonsPanel.Size = new System.Drawing.Size(140, 41);
            this.ButtonsPanel.PanelCollapsed += new System.EventHandler(this.ButtonsPanel_PanelCollapsed);
            // 
            // PlayButton
            // 
            this.PlayButton.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.PlayButton.Location = new System.Drawing.Point(91, 85);
            // 
            // NextButton
            // 
            this.NextButton.Focusable = false;
            this.NextButton.Location = new System.Drawing.Point(75, 3);
            this.NextButton.Movable = false;
            this.NextButton.Size = new System.Drawing.Size(62, 35);
            this.NextButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MouseEnter += new System.EventHandler(this.ButtonsPanel_PanelCollapsed);

            // 
            // BackButton
            // 
            this.BackButton.Focusable = false;
            this.BackButton.Location = new System.Drawing.Point(6, 3);
            this.BackButton.Size = new System.Drawing.Size(63, 35);
            this.BackButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MouseEnter += new System.EventHandler(this.ButtonsPanel_PanelCollapsed);

            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(47, 44);
            this.ExitButton.Visible = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            this.MouseEnter += new System.EventHandler(this.ButtonsPanel_PanelCollapsed);

            // 
            // TitlePanel
            // 
            this.TitlePanel.Dock = Wisej.Web.DockStyle.None;
            this.TitlePanel.Size = new System.Drawing.Size(336, 41);
            this.MouseEnter += new System.EventHandler(this.ButtonsPanel_PanelCollapsed);
            // 
            // TourPanel1
            // 
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MaximumSize = new System.Drawing.Size(501, 500);
            this.MinimumSize = new System.Drawing.Size(0, 267);
            this.Name = "TourPanel1";
            this.Size = new System.Drawing.Size(501, 267);
            this.Playing += new System.EventHandler(this.TourPanel1_Playing);
            this.ButtonsPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
