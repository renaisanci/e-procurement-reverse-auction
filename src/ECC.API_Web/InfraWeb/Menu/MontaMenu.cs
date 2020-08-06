using System.Collections.Generic;
using System.Linq;
using ECC.API_Web.InfraWeb.Menu;
using ECC.API_Web.Models;

namespace ECC.API_Web.InfraWeb
{
    public class MontaMenu : List<MenuAux>
    {

        public MontaMenu()
        {
        }

        public MontaMenu(List<MenuViewModel> lmenu)
        {

            foreach (var menu in lmenu)
            {
                if (menu.MenuPaiId > 0)
                {
                    MenuAux hmpai = this.BuscaMenu(menu.MenuPaiId);
                    MenuAux hm = new MenuAux();
                    hm.Id = menu.Id;
                    hm.Url = menu.Url;
                    hm.DescMenu = menu.DescMenu;
                    hm.MenuPaiId = menu.MenuPaiId;
                    hm.FontIcon = menu.FontIcon;
                    hm.Feature1 = menu.Feature1;
                    hm.Feature2 = menu.Feature2;
                    hm.Nivel = menu.Nivel;
                    hm.Ordem = menu.Ordem;
                    hm.ModuloId = menu.ModuloId;
                    
                    hmpai.Children.Add(hm);
                }
                else
                {
                    MenuAux hm = new MenuAux();
                    hm.Id = menu.Id;
                    hm.Url = menu.Url;
                    hm.DescMenu = menu.DescMenu;
                    hm.MenuPaiId = menu.MenuPaiId;
                    hm.FontIcon = menu.FontIcon;
                    hm.Feature1 = menu.Feature1;
                    hm.Feature2 = menu.Feature2;
                    hm.Nivel = menu.Nivel;
                    hm.Ordem = menu.Ordem;
                    hm.ModuloId = menu.ModuloId;


                    this.Add(hm);
                    this.ToList().OrderBy(p => p.Ordem);
                }
            }

            //return _lHierarquiaMenu;
        }

        public MenuAux BuscaMenu(int menuPaiId)
        {
            return BuscaMenu(this, menuPaiId);
        }

        public MenuAux BuscaMenu(MontaMenu lMenu, int menuPaiId)
        {
            foreach (var menu in lMenu.OrderBy(p=>p.Ordem))
            {
                if (menu.Id == menuPaiId)
                {
                    return menu;
                }
                else if (menu.Children.Count > 0)
                {
                    MenuAux hm = BuscaMenu(menu.Children, menuPaiId);
                    if (hm != null)
                    {
                        return hm;
                    }
                }
            }

            return null;
        }

        public string CriaMenu()
        {
            string sMenu = "";
            CriaMenu(this, ref sMenu, 0);
            return sMenu;
        }

        public void CriaMenu(MontaMenu _lMenuHieraquia, ref string sMenu, int ColocaUL)
        {

            foreach (var menu in _lMenuHieraquia)
            {
                if (string.IsNullOrEmpty(sMenu))
                {
                    sMenu = "<li>";
                }
                else
                {
                    sMenu += "<li>";
                }
                sMenu += @"<a href=""";
                sMenu += menu.Url;
                sMenu += @""">";
                sMenu += menu.Url;
                sMenu += "</a>";
                if (menu.Children.Count > 0)
                {
                    sMenu += @"	<ul class=""dl-submenu"">";
                    CriaMenu(menu.Children, ref sMenu, 1);
                }
                sMenu += "</li>";
            }

            if (ColocaUL == 1)
            {
                sMenu += "</ul>";
            }
        }

    }
}