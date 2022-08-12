using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace III_ProjectOne
{
    class LabelText
    {
        //Method to update the label text
        public static void UpdateText(Label label,string labelMessage)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => label.Text = labelMessage));
                label.Invoke(new Action(() => label.Refresh()));
            }

            else
            {
                label.Text = labelMessage;
                label.Refresh();
            }
               

        }
    }
}
