using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Animal
    {
        public abstract string Speak();
    }

    public class Cow : Animal
    {
        public override string Speak()
        {
            return "Moo";
        }
    }

    public class Pig : Animal
    {
        public override string Speak()
        {
            return "Oink";
        }
    }

    public class Farm : MonoBehaviour
    {
        public void Start()
        {
            Animal[] animals = { new Cow(), new Pig() };

            foreach (var animal in animals)
            {
                Debug.LogFormat("Some animal says '{0}'", animal.Speak());
            }
                
            var cow = new Cow();
            Debug.LogFormat("The cow says '{0}'", cow.Speak());
        }
    }
}
