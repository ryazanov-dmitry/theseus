using System;
using System.Collections.Generic;

namespace Theseus.Demo.Renderer
{
    public class WorldModel : IUpdatable
    {
        private readonly Dictionary<int, string> matrix;
        private readonly string ground = "_";
        private readonly string courier = "C";
        private readonly string verifier = "V";
        private readonly string client = "X";
        private readonly List<Subject> subjects;
        private readonly float min;
        private readonly float max;

        public WorldModel(float min, float max, Ticker ticker, List<Subject> subjects)
        {
            this.max = max;
            this.min = min;
            this.subjects = subjects;
            this.matrix = new Dictionary<int, string>();

            InitModel();

            ticker.Subscribe(this);
        }


        public void Update(long deltaT)
        {
            ClearModel();
            foreach (var subject in subjects)
            {
                var discreteCoord = (int)Math.Round(subject.Coordinates.X);
                matrix[discreteCoord] = RenderSubject(subject.SubjectType);
            }
            UpdateConsole();
        }

        private void UpdateConsole()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("              ");
            Console.Write(ConvertToString(matrix));
        }

        private string ConvertToString(Dictionary<int, string> matrix)
        {
            var result = "";

            foreach (var cell in matrix)
            {
                result += cell.Value;
            }

            return result;
        }

        private string RenderSubject(SubjectType subjectType)
        {
            switch (subjectType)
            {
                case SubjectType.Courier:
                    return courier;
                case SubjectType.Verifier:
                    return verifier;
                case SubjectType.Client:
                    return client;

                default: throw new Exception("Invalid subject type.");
            }
        }

        private void InitModel()
        {
            for (int i = (int)min; i < max; i++)
            {
                matrix.Add(i, ground);
            }
        }

        private void ClearModel()
        {
            for (int i = (int)min; i < max; i++)
            {
                matrix[i] = ground;
            }
        }
    }
}