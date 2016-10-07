﻿using Modulo1.Dal;
using Modulo1.Modelo;
using Plugin.Media;
using System;
using System.IO;

using Xamarin.Forms;

namespace Modulo1.Pages.Garcons
{
    public partial class GarconsNewPage : ContentPage
    {
        private byte[] bytesFoto;

        public GarconsNewPage()
        {
            InitializeComponent();
            RegistraClickBotaoAlbum();
        }

        private void RegistraClickBotaoAlbum()
        {
            BtnAlbum.Clicked += async (sender, args) =>
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    DisplayAlert("Álbum não suportado", "Não existe permissão para acessar o álbum de fotos", "OK");
                    return;
                }
                var file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null)
                    return;

                var stream = file.GetStream();
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                fotogarcom.Source = ImageSource.FromStream(() =>
                {
                    var s = file.GetStream();
                    file.Dispose();
                    return s;
                });
                bytesFoto = memoryStream.ToArray();
            };
        }

        private async void BtnGravarClicked(object sender, EventArgs e)
        {

            var dal = new GarcomDAL();
            var garcom = new Garcom();
            garcom.Nome = nome.Text;
            garcom.Sobrenome = sobrenome.Text;
            garcom.Foto = bytesFoto;
            dal.Add(garcom);
            ClearControls();
            await App.Current.MainPage.DisplayAlert("Inserção de garçom", "Garçom inserido com sucesso", "Ok");
        }

        private void ClearControls()
        {
            nome.Text = string.Empty;
            sobrenome.Text = string.Empty;
            bytesFoto = null;
            fotogarcom.Source = null;
        }
    }
}