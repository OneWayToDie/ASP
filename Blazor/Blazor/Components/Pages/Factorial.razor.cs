using System.Numerics;

namespace Blazor.Components.Pages
{
	public partial class Factorial
	{
		int limit = 0;
		BigInteger factorial = 1;
		void Calculate()
		{
			factorial = 1;
			for (int i = 1; i <= limit; i++)
				factorial *= i;
		}
	
	}
}
