﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpotSync.Domain
{
    public static class Extensions
    {
        public static List<T> GetRandomNItems<T>(this List<T> source, int n)
        {
            Random random = new Random();

            List<T> list = new List<T>();

            for (int i = 0; i < n; i++)
            {
                list.Add(source.ElementAt(random.Next(0, source.Count - 1)));
            }
            return list;
        }
    }
}
