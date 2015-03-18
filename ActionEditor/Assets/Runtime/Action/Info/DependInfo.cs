using UnityEngine;
using System.Collections.Generic;

namespace Action
{
    public class DependInfo : Dictionary<DependType, object>, IDependInfo
    {
        private IList<GameObject> _actors;

        private IList<Vector3> _points;

        public DependInfo() : base()
        {
            _actors = new List<GameObject>(new GameObject[actorLimit]);
            _points = new List<Vector3>(new Vector3[pointLimit]);
        }

        public IList<GameObject> actors
        {
            get
            {
                return _actors;
            }
        }

        public IList<Vector3> points
        {
            get
            {
                return _points;
            }
        }

        public virtual int actorLimit
        {
            get
            {
                return 11;
            }
        }

        public virtual int pointLimit
        {
            get
            {
                return actorLimit + 1;
            }
        }

        public object FindActor(string indexOrRange)
        {
            return FindIndexOrRange<GameObject>(actors, indexOrRange);
        }

        public void SetActor(string indexOrRange, object actor)
        {
            SetIndexOrRange<GameObject>(actors, indexOrRange, actor);
        }

        public object FindPoint(string indexOrRange)
        {
            return FindIndexOrRange<Vector3>(points, indexOrRange);
        }

        public void SetPoint(string indexOrRange, object point)
        {
            SetIndexOrRange<Vector3>(points, indexOrRange, point);
        }

        public static object FindIndexOrRange<T>(IList<T> objects, string indexOrRange)
        {
            object result = null;
            if (indexOrRange.StartsWith("[") && indexOrRange.EndsWith("]"))
            {
                string[] controls = indexOrRange.Substring(1, indexOrRange.Length - 2).Split(new string[] { ".." }, System.StringSplitOptions.RemoveEmptyEntries);
                int start = controls.Length >= 1 ? int.Parse(controls[0]) : 0;
                int end = controls.Length >= 2 ? int.Parse(controls[1]) : objects.Count - 1;
                end = Mathf.Max(end, objects.Count - 1);
                List<T> deps = new List<T>();
                for (int i = start; i <= end; i++)
                {
                    if (objects[i] != null)
                    {
                        deps.Add(objects[i]);
                    }
                }
                if (deps.Count > 0)
                {
                    result = deps.ToArray();
                }
            }
            else
            {
                int index = int.Parse(indexOrRange);
                if (index >= 0 && index < objects.Count)
                {
                    result = objects[index];
                }
            }
            return result;
        }

        public void SetIndexOrRange<T>(IList<T> objects, string indexOrRange, object value)
        {
            if (indexOrRange.StartsWith("[") && indexOrRange.EndsWith("]"))
            {
                string[] controls = indexOrRange.Substring(1, indexOrRange.Length - 2).Split(new string[] { ".." }, System.StringSplitOptions.RemoveEmptyEntries);
                int start = controls.Length >= 1 ? int.Parse(controls[0]) : 0;
                int end = controls.Length >= 2 ? int.Parse(controls[1]) : objects.Count - 1;
                List<T> deps = value as List<T>;
                end = Mathf.Min(end, deps.Count + start - 1);
                for (int i = start; i <= end; i++)
                {
                    objects[i] = deps[i - start];
                }
            }
            else
            {
                int index = int.Parse(indexOrRange);
                if (index >= 0 && index < objects.Count)
                {
                    objects[index] = (T)value;
                }
            }
        }
    }
}
