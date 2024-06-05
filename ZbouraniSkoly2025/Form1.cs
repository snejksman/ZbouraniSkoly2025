﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZbouraniSkoly2025
{
    public partial class Form1 : Form
    {
        // hlavni nastroje pro 
        Bitmap mobjMainBitmap;
        Graphics mobjBitmapGraphics;

        // grafika pro picturebox
        Graphics mobjPlatnoGraphics;

        // vytvoreni kulicky
        clsKulicka mobjBall;

        // vytvoreni plosiny
        clsPlosina mobjPlosina;

        // vytvoreni cihly
        clsCihla mobjCihla;

        // mackam klavesnici
        public bool mbjOvladam;

        // veci pro mazani cihel
        bool mbjCihlaNeni;
        int mintRectCislo;

        // veci pro konec hry
        bool mbjWin;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // vytvoreni grafiky v pictureboxu
            mobjPlatnoGraphics = pbPlatno.CreateGraphics();

            // vytvoreni hlavni grafiky
            mobjMainBitmap = new Bitmap(pbPlatno.Width, pbPlatno.Height);
            mobjBitmapGraphics = Graphics.FromImage(mobjMainBitmap);

            // nastaveni kulicky
            mobjBall = new clsKulicka(600, 350, 13, 4, 4, mobjBitmapGraphics);

            // nastaveni plosiny
            mobjPlosina = new clsPlosina((int)(mobjPlatnoGraphics.VisibleClipBounds.Width / 2), 500, 100, 10, 8, mobjBitmapGraphics);

            // nastaveni cihel
            mobjCihla = new clsCihla(8, 5, 20, 20, 120, 40, 20, 10, mobjBitmapGraphics);

            // nastaveni timeru
            tmrRedraw.Interval = 30;
            tmrRedraw.Enabled = true;
            mbjWin = false;
        }

        //
        // prekresleni obrazu pri ticku timeru
        //
        private void tmrRedraw_Tick(object sender, EventArgs e)
        {
            // vymazat platno a grafiku
            mobjBitmapGraphics.Clear(Color.White);

            // nakresli kulicku na bitmapu a jeji kolize
            mobjBall.DrawBall();
            mobjBall.MoveBall();
            mobjBall.KolizeBall();

            // nakresli plosinu a jeji kolize
            mobjPlosina.DrawPlosina();
            if (mbjOvladam == true)
            {
                mobjPlosina.MovePlosina();
            }
            mobjBall.KolizeBallAndPlosina(mobjPlosina.pintPlosinaX, mobjPlosina.pintPlosinaY, mobjPlosina.pintPlosinaWidth);

            // nakresli cihly
            mobjCihla.DrawCihla();
            TestKolizeBallCihla();

            // nakresleni na platno
            mobjPlatnoGraphics.DrawImage(mobjMainBitmap, 0, 0);
            if (mobjBall.tmrGameOver == true)
            {
                tmrRedraw.Enabled = false;
                GameOver();
            }
            if (mobjCihla.mbjGameWin == true)
            {
                tmrRedraw.Enabled = false;
                GameWin();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs Klavesa)
        {
            try
            {
                switch (Klavesa.KeyCode)
                {
                    case Keys.Left:
                        mobjPlosina.PosunLeft();
                        if (mobjPlosina.pintPlosinaX < 0)
                        {
                            mbjOvladam = false;
                        }
                        else 
                        { 
                            mbjOvladam = true; 
                        }
                        break;
                    case Keys.Right:
                        mobjPlosina.PosunRight();
                        if (mobjPlosina.pintPlosinaX + mobjPlosina.pintPlosinaWidth > mobjPlatnoGraphics.VisibleClipBounds.Width)
                        {
                            mbjOvladam = false;
                        }
                        else
                        {
                            mbjOvladam = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                mbjOvladam = false;
            }
        }

        // zrusi pohyb
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            mbjOvladam = false;
        }

        //
        // jina kolize kulicky s cihlou - ma to byt jednodussi ale proste to nefunguje a nemam tuseni proc kdyz to funovat ma 
        //
        //                      |
        //                      V
        //
        // update - uz to funguje
        //
        private void TestKolizeBallCihla()
        {

            // plati pro kazdy rect v listu
            foreach (Rectangle rect in mobjCihla.listRect)
            {
                Rectangle lobjPrekryv;

                // lobjprekryv je rectangle ktery je prostor prekryti dvou jinych rectanglu
                lobjPrekryv = Rectangle.Intersect(rect, mobjBall.mobjBallRect);

                // kdyz jsou rozmery prekryvu vetsi jak 0, zaznamena se kolize
                if (lobjPrekryv.Width > 0 && lobjPrekryv.Height > 0)
                {
                    mobjBall.mintBallPosunY = mobjBall.mintBallPosunY * (-1);
                    mobjBall.mintBallPosunX = mobjBall.mintBallPosunX + mobjBall.mintRandomPosun;

                    // smaze jeden z rectanglu z listu
                    mintRectCislo = mobjCihla.listRect.IndexOf(rect);
                    mbjCihlaNeni = true;

                    // kontroluje pocet zbyvajicich cihel
                    if (mobjCihla.pintPocetCihel < 2)
                    {
                        mbjWin = true;
                    }
                }
            }

            // odstrani jeden rectangle z listu
            if (mbjCihlaNeni == true)
            {
                mobjCihla.listRect.RemoveAt(mintRectCislo);
                mbjCihlaNeni = false;
            }
            
        }

        // popup okynko s koncem hry
        public void GameOver()
        {
            DialogResult Restartovat = MessageBox.Show("chces bourat znova?", "skola porad stoji (prohra)", MessageBoxButtons.YesNo);
            switch (Restartovat)
            {
                case DialogResult.Yes:
                    Application.Restart();
                    break;
                case DialogResult.No:
                    this.Close();
                    break;
            }
        }
        public void GameWin()
        {
            DialogResult Restartovat = MessageBox.Show("chces bourat znova?", "skola uz nestoji (vyhra)", MessageBoxButtons.YesNo);
            switch (Restartovat)
            {
                case DialogResult.Yes:
                    Application.Restart();
                    break;
                case DialogResult.No:
                    this.Close();
                    break;
            }
        }
    }
}
