using System;
using System.Drawing;
using Wisej.Web;
using Wisej.Web.Ext.TourPanel;
using Wisej.Web.Markup;
using Wisej.Web.Ext.RibbonBar;

namespace OverjoyedReleaseLocal
{
    public partial class GuidedTour : TourPanel
    {
        public GuidedTour()
        {
            InitializeComponent();  

            // Calculate the preferred height based on the text content.
        }

        //private void ScaleFonts(Control parent, float scale)
        //{
        //    foreach (Control ctrl in parent.Controls)
        //    {
        //        // Scale the font size
        //        ctrl.Font = new Font(
        //            ctrl.Font.Name,
        //            ctrl.Font.Size * scale,
        //            ctrl.Font.Style
        //        );

        //        // Recursively scale fonts in child controls
        //        if (ctrl.HasChildren)
        //            ScaleFonts(ctrl, scale);
        //    }
        //}



        private void ButtonsPanel_PanelCollapsed(object sender, EventArgs e)
        {
            Eval(@"function applyMarquee(el) {

  if (el.dataset.marqueeApplied === 'true' && el.querySelector('.marquee-outer')) return;
  let text = el.textContent.trim();
  if (!text) return;

el.style.whiteSpace = 'nowrap'; // already in CSS

  let cs = getComputedStyle(el);
  let wasCentered = cs.textAlign === 'center';

  el.textContent = '';
  let outer = document.createElement('span');
  outer.className = 'marquee-outer';
  outer.style.display = 'block';
  outer.style.width = '100%';

  let inner = document.createElement('span');
  inner.className = 'marquee-inner';

  let s1 = document.createElement('span');
  s1.textContent = text;
  let s2 = document.createElement('span');
  s2.textContent = text;

  inner.appendChild(s1);
  inner.appendChild(s2);
  outer.appendChild(inner);
  el.appendChild(outer);

  s1.style.display = 'inline-flex'; // already in CSS
if (wasCentered) {
    let space = el.clientWidth - s1.offsetWidth;
    s1.style.paddingLeft = (space/2) + 'px';
}

  

  el.dataset.marqueeApplied = 'true';
}


document.querySelectorAll('[name^=""TitleLabel""]').forEach(el => {
applyMarquee(el);
});
document.querySelectorAll('[name^=""label""]').forEach(el => {
applyMarquee(el);
});
");



          
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {

        }

        private void HtmlText_Appear(object sender, EventArgs e)
        {
        }

        private void TourPanel1_Playing(object sender, EventArgs e)
        {
        }
    }
}
