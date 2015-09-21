using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class AVIncrementOperation : IAVFieldOperation
	{
	    private static readonly IDictionary<Tuple<Type, Type>, Func<object, object, object>> adders =
	        (IDictionary<Tuple<Type, Type>, Func<object, object, object>>)
	            new Dictionary<Tuple<Type, Type>, Func<object, object, object>>()
	            {
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (sbyte)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (sbyte) left + (int) (sbyte) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (short)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (sbyte) left + (int) (short) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (int)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (sbyte) left + (int) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (sbyte) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (sbyte) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (sbyte) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (sbyte), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((sbyte) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (byte)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (byte) left + (int) (byte) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (short)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (byte) left + (int) (short) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (ushort)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (byte) left + (int) (ushort) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (int)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (byte) left + (int) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (uint)),
	                    (Func<object, object, object>) ((left, right) => (object) ((uint) (byte) left + (uint) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (byte) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (ulong)),
	                    (Func<object, object, object>) ((left, right) => (object) ((ulong) (byte) left + (ulong) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (byte) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (byte) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (byte), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((byte) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (short), typeof (short)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (short) left + (int) (short) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (short), typeof (int)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (short) left + (int) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (short), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (short) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (short), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (short) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (short), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (short) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (short), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((short) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (ushort)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (ushort) left + (int) (ushort) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (int)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (ushort) left + (int) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (uint)),
	                    (Func<object, object, object>) ((left, right) => (object) ((uint) (ushort) left + (uint) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (ushort) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (ulong)),
	                    (Func<object, object, object>) ((left, right) => (object) ((ulong) (ushort) left + (ulong) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (ushort) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (double)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((double) (ushort) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ushort), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((ushort) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (int), typeof (int)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) left + (int) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (int), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (int) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (int), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (int) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (int), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (int) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (int), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((int) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (uint), typeof (uint)),
	                    (Func<object, object, object>) ((left, right) => (object) ((uint) left + (uint) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (uint), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (uint) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (uint), typeof (ulong)),
	                    (Func<object, object, object>) ((left, right) => (object) ((ulong) (uint) left + (ulong) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (uint), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (uint) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (uint), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (uint) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (uint), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((uint) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (long), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (long), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (long) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (long), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (long) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (long), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((long) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (char)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (char) left + (int) (char) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (ushort)),
	                    (Func<object, object, object>) ((left, right) => (object) ((int) (char) left + (int) (char) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (int)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (char) left + (int) (ushort) (int) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (uint)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((int) (char) left + (int) (ushort) (uint) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (long)),
	                    (Func<object, object, object>) ((left, right) => (object) ((long) (char) left + (long) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (ulong)),
	                    (Func<object, object, object>) ((left, right) => (object) ((ulong) (char) left + (ulong) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (char) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (char) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (char), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ((decimal) ((char) left) + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (float), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (float), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (float) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ulong), typeof (ulong)),
	                    (Func<object, object, object>) ((left, right) => (object) ((ulong) left + (ulong) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ulong), typeof (float)),
	                    (Func<object, object, object>) ((left, right) => (object) ((float) (ulong) left + (float) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ulong), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) (ulong) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (ulong), typeof (decimal)),
	                    (Func<object, object, object>)
	                        ((left, right) => (object) ( (ulong) left + (decimal) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (double), typeof (double)),
	                    (Func<object, object, object>) ((left, right) => (object) ((double) left + (double) right))
	                },
	                {
	                    new Tuple<Type, Type>(typeof (decimal), typeof (decimal)),
	                    (Func<object, object, object>) ((left, right) => (object) ((decimal) left + (decimal) right))
	                }
	            };
        private object amount;

		public object Amount
		{
			get
			{
				return amount;
			}
		}

        static AVIncrementOperation()
        {
            foreach (Tuple<Type, Type> index1 in adders.Keys.ToList<Tuple<Type, Type>>())
            {
                if (!index1.Item1.Equals(index1.Item2))
                {
                    Tuple<Type, Type> index2 = new Tuple<Type, Type>(index1.Item2, index1.Item1);
                    Func<object, object, object> item = adders[index1];
                    adders[index2] = (Func<object, object, object>)((left, right) => item(right, left));
                }
            }
        }

        public AVIncrementOperation(object amount)
		{
			this.amount = amount;
		}

		private static object Add(object obj1, object obj2)
		{
            Func<object, object, object> func;
            if (adders.TryGetValue(new Tuple<Type, Type>(obj1.GetType(), obj2.GetType()), out func))
                return func(obj1, obj2);
		    throw new InvalidCastException(string.Concat(new object[4]
		    {
		        "Cannot add ",
		        obj1.GetType(),
		        " to ",
		        obj2.GetType()
		    }));
		}

		public object Apply(object oldValue, AVObject obj, string key)
		{
			if (oldValue is string)
			{
				throw new InvalidOperationException("Cannot increment a non-number type.");
			}
			return Add(oldValue ?? 0, amount);
		}

		public object Encode()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__op", "Increment");
			dictionary.Add("amount", amount);
			return dictionary;
		}

		public IAVFieldOperation MergeWithPrevious(IAVFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is AVDeleteOperation)
			{
				return new AVSetOperation(amount);
			}
			if (!(previous is AVSetOperation))
			{
				if (!(previous is AVIncrementOperation))
				{
					throw new InvalidOperationException("Operation is invalid after previous operation.");
				}
				object amount = ((AVIncrementOperation)previous).Amount;
				return new AVIncrementOperation(Add(amount, this.amount));
			}
			object value = ((AVSetOperation)previous).Value;
			if (value is string)
			{
				throw new InvalidOperationException("Cannot increment a non-number type.");
			}
			return new AVSetOperation(Add(value, this.amount));
		}
	}
}