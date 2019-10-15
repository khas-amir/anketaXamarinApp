using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Xml;
using System.Collections.Generic;
using System.Net;

namespace XamarinTut
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        XmlElement xRoot;
        XmlDocument xmlDoc;
        private int status = 0;
        private string next;
        private List<string> answ = new List<string>(), next_arr = new List<string>();
        private List<RadioButton> array = new List<RadioButton>();
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private RadioGroup radioGroup;
        private TextView txtView;
        private Button nextBtn;        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            #region Load XML
            //Загрузка вопросов
            string content;            
            using (System.IO.StreamReader sr = new System.IO.StreamReader(Assets.Open("text.xml")))
            {
                content = sr.ReadToEnd();
            }
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            xRoot = xmlDoc.DocumentElement;
            #endregion

            #region Binding
            radioButton1 = FindViewById<RadioButton>(Resource.Id.radioButton1);
            radioButton2 = FindViewById<RadioButton>(Resource.Id.radioButton2);
            radioButton3 = FindViewById<RadioButton>(Resource.Id.radioButton3);
            radioButton4 = FindViewById<RadioButton>(Resource.Id.radioButton4);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            txtView = FindViewById<TextView>(Resource.Id.textView1);
            nextBtn = FindViewById<Button>(Resource.Id.button1);
            #endregion

            radioButton1.Visibility = Android.Views.ViewStates.Gone;
            radioButton2.Visibility = Android.Views.ViewStates.Gone;
            radioButton3.Visibility = Android.Views.ViewStates.Gone;

            txtView.Text = "Первая медицинская помощь пострадавшему на воде";

            nextBtn.Click += (sender, args) =>
            {
                string param = next;
                switch (status)
                {
                    case 0:
                        {
                            radioButton1.Visibility = Android.Views.ViewStates.Visible;
                            radioButton2.Visibility = Android.Views.ViewStates.Visible;
                            radioButton3.Visibility = Android.Views.ViewStates.Visible;
                            Find("1");
                            status = 1;
                            nextBtn.Enabled = false;
                            radioButton4.Checked = true;
                            break;
                        }
                    case 1:
                        {
                            next_arr.Clear();
                            answ.Clear();
                            Find(param);
                            break;
                        }
                    case 2:
                        {
                            this.FinishAffinity();
                            break;
                        }
                }
            };
            
            radioGroup.CheckedChange += (sender, args) =>
            {
                if (radioButton1.Checked) next = next_arr[0];
                if (radioButton2.Checked) next = next_arr[1];
                if (radioButton3.Checked) next = next_arr[2];
                nextBtn.Enabled = true;
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Find(string next)
        {
            if (next[0] != 's')
            {
                foreach (XmlNode answer in xRoot.SelectNodes(string.Format("//q[@num='{0}']/a", next)))
                {
                    answ.Add(answer.InnerText);
                }
                ShowQw(xRoot.SelectSingleNode(string.Format("//qw/q[@num='{0}']", next)).SelectSingleNode("@text").Value);
                foreach (XmlNode ans_next in xRoot.SelectNodes(string.Format("//q[@num='{0}']/a", next)))
                {
                    next_arr.Add(ans_next.SelectSingleNode("@next").Value);
                }
            }
            else
            {
                string sol_text = xRoot.SelectSingleNode(string.Format("//solutions/sol[@num='{0}']", WebUtility.HtmlEncode(next))).InnerText,
                    sol_next = xRoot.SelectSingleNode(string.Format("//solutions/sol[@num='{0}']", WebUtility.HtmlEncode(next))).SelectSingleNode("@next").Value;
                // MessageBox.Show(sol_text, "Решение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Решение");
                alert.SetMessage(sol_text);
                alert.SetButton("OK", (c,v) => {
                    alert.Dismiss();
                });
                alert.Show();
                if (sol_next != "end")
                    Find(sol_next);
                else
                {
                    status = 2;
                    ShowEnd();
                }
            }
        }

        private void ShowEnd()
        {
            txtView.Text = "Опрос завершен";
            radioButton1.Visibility = Android.Views.ViewStates.Gone;
            radioButton2.Visibility = Android.Views.ViewStates.Gone;
            radioButton3.Visibility = Android.Views.ViewStates.Gone;
        }

        private void ShowQw(string str)
        {
            txtView.Text = str;
            array.Clear();
            radioButton1.Text = answ[0];
            radioButton2.Text = answ[1];
            radioButton1.Visibility = Android.Views.ViewStates.Visible;
            radioButton2.Visibility = Android.Views.ViewStates.Visible;
            radioButton3.Visibility = Android.Views.ViewStates.Visible;
            try
            {
                radioButton3.Text = answ[2];
            }
            catch
            {
                radioButton3.Visibility = Android.Views.ViewStates.Gone;
            }

            radioButton4.Checked = true;
            nextBtn.Enabled = false;
        }
    }

}