using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CEAngela.AI
{
    class EvaluationCore
    {
        private float[][] _values;
        private float[][] _bias;
        private float[][][] _weights;

        private string _path = @"Evaluation.txt";

        public EvaluationCore(IReadOnlyList<int> _structure)
        {
            var _random = new System.Random();

            _values = new float[_structure.Count][];
            _bias = new float[_structure.Count][];
            _weights = new float[_structure.Count - 1][][];

            for (int i = 0; i < _structure.Count; i++)
            {
                _values[i] = new float[_structure[i]];
                _bias[i] = new float[_structure[i]];
            }

            if(!File.Exists(_path))
            {
                for (int i = 0; i < _structure.Count - 1; i++)
                {
                    _weights[i] = new float[_values[i + 1].Length][];

                    for (int x = 0; x < _weights[i].Length; x++)
                    {
                        _weights[i][x] = new float[_values[i].Length];

                        for (int y = 0; y < _weights[i][x].Length; y++)
                            _weights[i][x][y] = (float)_random.NextDouble() * (float)Math.Sqrt(2f / _weights[i][x].Length);
                    }
                }
            }
            else
            {
                var _file = File.OpenRead(_path);
                var _streamReader = new StreamReader(_file);

                for (int i = 0; i < _structure.Count - 1; i++)
                {
                    _weights[i] = new float[_values[i + 1].Length][];

                    for (int x = 0; x < _weights[i].Length; x++)
                    {
                        _weights[i][x] = new float[_values[i].Length];

                        for (int y = 0; y < _weights[i][x].Length; y++)
                        {
                            var _line = _streamReader.ReadLine();
                            _weights[i][x][y] = _line == null ? (float)_random.NextDouble() * (float)Math.Sqrt(2f / _weights[i][x].Length) : float.Parse(_line);
                        }
                    }
                }
            }

            if (!File.Exists(_path))
            {
                var _file = File.Create(_path);
                var _streamWriter = new StreamWriter(_file);

                for (int i = 0; i < _structure.Count - 1; i++)
                    for (int x = 0; x < _weights[i].Length; x++)
                        for (int y = 0; y < _weights[i][x].Length; y++)
                        {
                            _weights[i][x][y] = (float)_random.NextDouble() * (float)Math.Sqrt(2f / _weights[i][x].Length);

                            _streamWriter.WriteLine(_weights[i][x][y]);
                        }

                _file.Close();
            }
        }

        public float[] Calculate(float[] _inputs)
        {
            for (int i = 0; i < _values[0].Length; i++)
            {
                _values[0][i] = _inputs[i];
            }     

            for (int i = 1; i < _values.Length; i++)
            {
                for (int x = 0; x < _values[i].Length; x++)
                {
                    for (int y = 0; y < _values[i - 1].Length; y++)
                    {
                        _values[i][x] += _values[i - 1][y] * _weights[i - 1][x][y];
                    }
                    _values[i][x] += _bias[i][x];
                }
            }

            return _values[_values.Length - 1];
        }
    }
}
