using Imml;
using Imml.ComponentModel;
using Imml.Runtime;
using Imml.Runtime.Services;
using ImmlSharp.StereoKit.Services;
using StereoKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SK = StereoKit;

namespace ImmlSharp.StereoKit
{
    public static class ImmlExtensions
    {
        public static void LoadMaterials(this IMaterialHostElement materialHostElement, SK.Model model, ICacheService cacheService)
        {
            var materialGroups = materialHostElement.GetMaterialGroups();

            foreach (var materialGroup in materialGroups)
            {
                var material = materialGroup.GetMaterial();
                var texture = materialGroup.GetTexture();
                var videoTexture = materialGroup.GetVideo();
                var webTexture = materialGroup.GetWeb();

                if (material != null)
                {
                    //assign materials during draw instead of here
                    //var skMaterial = model.GetMaterial(0);
                    //if (materialGroup.Id == -1)
                    //{
                    //    model.SetMaterial(Material.FromColor(material.Diffuse.ToColor()));
                    //}
                    //else
                    //{
                    //    model.SetMaterial((uint)materialGroup.Id, Material.FromColor(material.Diffuse.ToColor()));
                    //}
                }

                if (texture != null)
                {
                    var fileExtension = texture.Source.FileExtension();
                    var hash = texture.Source.ToMD5() + fileExtension;
                    var path = System.IO.Path.Combine((cacheService as CacheService).CacheDir.Path, hash);

                    //TODO: This is causing SK to crash, so disabling for now
                    //var mat = model.GetMaterial(0);
                    //mat.SetTexture("diffuse", Tex.FromFile(path));
                }
            }
        }
    }
}
