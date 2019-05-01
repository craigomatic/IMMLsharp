using Imml;
using Imml.Runtime;
using Imml.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Audio;

namespace IMMLSharp.Scene.Controls
{
    public class Sound : Imml.Scene.Controls.Sound, IRuntimeElement<Node>
    {
        public Node Node { get; private set; }

        public async Task AcquireResourcesAsync()
        {
            var resourceAcquisitionService = DIContainer.Get<IResourceAcquisitionService>();

            try
            {
                //just acquire the resource, which will drop it into the resource cache so it can be used later during load
                await resourceAcquisitionService.AcquireResource(this);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load {this.GetType().Name} resource from {this.Source}");
            }
        }

        public void ApplyLayout()
        {
            this.Node.SetWorldPosition(this.WorldPosition.ToUrhoVector3());
            this.Node.SetWorldRotation(this.WorldRotation.ToQuaternion());
        }

        public void Dispose()
        {
            this.Node.Remove();
            this.Node.Dispose();
        }

        public Node Load(Node parentNode)
        {
            this.Node = parentNode.CreateChild(this.Name);

            this.LoadPositionalElement(this.Node);

            SoundSource soundSource = null;

            if (this.Spatial)
            {
                soundSource = this.Node.CreateComponent<SoundSource3D>();
            }
            else
            {
                soundSource = this.Node.CreateComponent<SoundSource>();
            }

            var resourceCache = DIContainer.Get<Urho.Resources.ResourceCache>();

            var fileExtension = this.Source.FileExtension();
            var hash = this.Source.ToMD5() + fileExtension;
            var sound = resourceCache.GetSound(hash);
            sound.Looped = this.Loop;
            soundSource.Enabled = this.Enabled;

            if (this.Enabled)
            {
                soundSource.SetSoundType(Urho.SoundType.Effect.ToString());

                soundSource.Play(sound);
                soundSource.Gain = this.Volume;
            }

            return this.Node;
        }
    }
}
