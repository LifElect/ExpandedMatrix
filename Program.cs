using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;

class MatrixMatchException : Exception
{
    public MatrixMatchException(string message) : base(message) { }
}

class MatrixInverseException : Exception
{
    public MatrixInverseException(string message) : base(message) { }
}

class MatrixExistException : Exception
{
    public MatrixExistException(string message) : base(message) { }
}

delegate void DiagonalizeMatrixDelegate(int[,] matrix);
delegate void CheckOperationChoose(int choose);

class ExpandedMatrix : SquareMatrix
{
    public SquareMatrix TransposeMatrix()
    {
        int[,] TransposedMatrix = new int[size, size];
        for (int CurrentNumberX = 0; CurrentNumberX < size; ++CurrentNumberX)
        {
            for (int CurrentNumberY = 0; CurrentNumberY < size; ++CurrentNumberY)
            {
                TransposedMatrix[CurrentNumberX, CurrentNumberY] = IntMatrix[CurrentNumberY, CurrentNumberX];
            }
        }
        return new SquareMatrix(TransposedMatrix);
    }

    public double MatrixTrace()
    {
        if (IntMatrix.GetLength(0) != IntMatrix.GetLength(1))
        {
            throw new ArgumentException("Matrix must be squared.");
        }
        double trace = 0;

        for (int CurrentDiagonalNumber = 0; CurrentDiagonalNumber < size; CurrentDiagonalNumber++)
        {
            trace += IntMatrix[CurrentDiagonalNumber, CurrentDiagonalNumber];
        }

        return trace;
    }

    public static void DiagonalizeMatrix(int[,] matrix)
    {
        if (matrix == null)
        {
            throw new MatrixExistException("Matrix don`t exist.");
        }

        for (int CurrentNumberX = 0; CurrentNumberX < matrix.GetLength(0); CurrentNumberX++)
        {
            for (int CurrentNumberY = 0; CurrentNumberY < matrix.GetLength(0); CurrentNumberY++)
            {
                if (CurrentNumberX != CurrentNumberY)
                {
                    matrix[CurrentNumberX, CurrentNumberY] = 0;
                }
            }
        }
    }
}

public abstract class IEvent
{
    public string EventType { get; set; }
}
class BasicMathOperations : IEvent
{
    public BasicMathOperations() { EventType = "Math"; }
}
class Comparison : IEvent
{
    public Comparison() { EventType = "Comparison"; }
}
class Inverse_Diagonalize_Transposed : IEvent
{
    public Inverse_Diagonalize_Transposed() { EventType = "MatrixOperations"; }
}
class Determinant_Trace_NonZero : IEvent
{
    public Determinant_Trace_NonZero() { EventType = "MatrixDatas"; }
}
class Clone : IEvent
{
    public Clone() { EventType = "Clone"; }
}
class GetHashCode : IEvent
{
    public GetHashCode() { EventType = "HashCode"; }
}
class Equals : IEvent
{
    public Equals() { EventType = "Equals"; }
}

class EventNull : IEvent
{
    public EventNull() { }
}

public abstract class BaseHandler
{
    DiagonalizeMatrixDelegate delegateInstance = delegate (int[,] m) { SquareMatrix.DiagonalizeMatrix(m); };

    public BaseHandler() { Next = null; }
    public virtual void Handle(IEvent ev)
    {

        if (PrivateEvent.EventType == ev.EventType)
        {
            SquareMatrix m1 = new SquareMatrix(3);
            SquareMatrix m2 = new SquareMatrix(3);

            Console.WriteLine("Matrix 1:");
            Console.Write(m1.ToString());

            Console.WriteLine("Matrix 2:");
            Console.Write(m2.ToString());

            Console.WriteLine("{0} successfully handled", PrivateEvent.EventType);
            if (PrivateEvent.EventType == "Math")
            {
                SquareMatrix m3 = m1 + m2;
                Console.WriteLine("Sum of Matrix 1 and Matrix 2:");
                Console.Write(m3.ToString());

                SquareMatrix m4 = m1 * m2;
                Console.WriteLine("Product of Matrix 1 and Matrix 2:");
                Console.Write(m4.ToString());

                int sum = (int)m1;
                Console.WriteLine("Sum of elements in Matrix 1: " + sum);
            }
            else if (PrivateEvent.EventType == "Comparison")
            {
                bool greater = m1 > m2;
                Console.WriteLine("Matrix 1 > Matrix 2: " + greater);

                bool less = m1 < m2;
                Console.WriteLine("Matrix 1 < Matrix 2: " + less);

                bool greaterOrEqual = m1 >= m2;
                Console.WriteLine("Matrix 1 >= Matrix 2: " + greaterOrEqual);

                bool lessOrEqual = m1 <= m2;
                Console.WriteLine("Matrix 1 <= Matrix 2: " + lessOrEqual);

                bool equal = m1 == m2;
                Console.WriteLine("Matrix 1 == Matrix 2: " + equal);

                bool notEqual = m1 != m2;
                Console.WriteLine("Matrix 1 != Matrix 2: " + notEqual);
            }
            else if (PrivateEvent.EventType == "MatrixOperations")
            {
                Console.WriteLine("Inverse of Matrix 1:");
                m1.InverseMatrix();

                SquareMatrix TrMatrix = m1.TransposeMatrix();
                Console.WriteLine("Transposed Matrix 1: ");
                Console.WriteLine(TrMatrix.ToString());

                Console.WriteLine("Diagonalized Matrix 1: ");
                SquareMatrix diagMatrix = (SquareMatrix)m1.Clone();
                delegateInstance(diagMatrix.GetMatrix());
                Console.WriteLine(diagMatrix.ToString());
            }
            else if (PrivateEvent.EventType == "MatrixDatas")
            {
                bool isNonZero = m1;
                Console.WriteLine("Matrix 1 is non-zero: " + isNonZero);

                double determinant = m1.Determinant();
                Console.WriteLine("Determinant of Matrix 1: " + determinant);

                double trace = m1.MatrixTrace();
                Console.WriteLine("Trace of Matrix 1: " + trace);
            }
            else if (PrivateEvent.EventType == "Clone")
            {
                SquareMatrix clone = (SquareMatrix)m1.Clone();
                Console.WriteLine("Clone of Matrix 1:");
                Console.Write(clone.ToString());
            }
            else if (PrivateEvent.EventType == "HashCode")
            {
                int hashCode = m1.GetHashCode();
                Console.WriteLine("HashCode of Matrix 1: " + hashCode);
            }
            else if (PrivateEvent.EventType == "Equals")
            {
                bool equalsClone = m1.Equals(m1.Clone());
                Console.WriteLine("Matrix 1 equals to its clone: " + equalsClone);
            }
        }
        else
        {
            Console.WriteLine("Sending event to next Handler...");
            if (Next != null)
                Next.Handle(ev);
            else
                Console.WriteLine("Unknown event. Can't handle.");

        }
    }
    protected void SetNextHandler(BaseHandler newHandler)
    {
        Next = newHandler;
    }
    protected BaseHandler Next { get; set; }
    protected IEvent PrivateEvent { get; set; }
}

public class ChainApplication
{
    public ChainApplication()
    {
        eventHandler = new Handler1();
    }
    public void Run(int EventCount)
    {
        bool IsTrue = true;
        while(IsTrue == true)
        {
            HandleEvent(ChooseEvent());
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("1 - continue \n2 - stop");
            int DoingGame = 0;
            while (true) {
                if (DoingGame == 1) { continue; }
                else if (DoingGame == 2) { IsTrue = false; }
                else { Console.WriteLine("Wrong input.");
                    continue;
                }
                        }

        }
    }


    private void HandleEvent(IEvent ev)
    {
        eventHandler.Handle(ev);
    }

    private IEvent ChooseEvent()
    {
        IEvent result = new EventNull();

        
            Console.WriteLine("Choose What you want to do: ");
            Console.WriteLine("1 - Basic math operations");
            Console.WriteLine("2 - Comparison matriсes");
            Console.WriteLine("3 - Inverse, Diagonalize, Transpoted views of Matrix 1");
            Console.WriteLine("4 - Determinant, Trace of Matrix 1 and check it`s not zero");
            Console.WriteLine("5 - Clone Matrix 1");
            Console.WriteLine("6 - Get HashCode of Matrix 1");
            Console.WriteLine("7 - Check if equals Matrix 1 and his clone");

            try
            {
                int Choose = Convert.ToInt32(Console.ReadLine());
                switch (Choose)
                {
                    case 1:
                        result = new BasicMathOperations();
                        break;
                    case 2:
                        result = new Comparison();
                        break;
                    case 3:
                        result = new Inverse_Diagonalize_Transposed();
                        break;
                    case 4:
                        result = new Determinant_Trace_NonZero();
                        break;
                    case 5:
                        result = new Clone();
                        break;
                    case 6:
                        result = new GetHashCode();
                        break;
                    case 7:
                        result = new Equals();
                        break;
                    default:
                        Console.WriteLine("Event was not found");
                        break;
                }
                if (Choose < 1 || Choose > 7)
                {
                    throw new ArgumentOutOfRangeException($"Number must be from 1 to 7.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка! Введенное значение не является числом.");   
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка! {ex.Message}");
            }
        
        return result;
    }
    private BaseHandler eventHandler;
}


class Handler7 : BaseHandler
{
    public Handler7()
    {
        PrivateEvent = new Equals(); Next = null;
    }
}
class Handler6 : BaseHandler
{
    public Handler6()
    {
        PrivateEvent = new GetHashCode(); Next = new Handler7();
    }
}
class Handler5 : BaseHandler
{
    public Handler5()
    {
        PrivateEvent = new Clone(); Next = new Handler6();
    }
}
class Handler4 : BaseHandler
{
    public Handler4()
    {
        PrivateEvent = new Determinant_Trace_NonZero(); Next = new Handler5();
    }
}
class Handler3 : BaseHandler
{
    public Handler3()
    {
        PrivateEvent = new Inverse_Diagonalize_Transposed(); Next = new Handler4();
    }
}
class Handler2 : BaseHandler
{
    public Handler2()
    {
        PrivateEvent = new Comparison(); Next = new Handler3();
    }
}
class Handler1 : BaseHandler
{
    public Handler1()
    {
        PrivateEvent = new BasicMathOperations(); Next = new Handler2();
    }
}

class SquareMatrix : ICloneable, IComparable<SquareMatrix>, IEquatable<SquareMatrix>
{
    protected readonly int size;
    protected int[,] IntMatrix;

    public SquareMatrix() { }

    public SquareMatrix(int size)
    {
        this.size = size;
        IntMatrix = new int[size, size];
        Random rand = new Random();

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                IntMatrix[currentNumberX, currentNumberY] = rand.Next(10);
            }
        }
    }

    public SquareMatrix(int[,] values)
    {
        size = values.GetLength(0);
        IntMatrix = new int[size, size];

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                IntMatrix[currentNumberX, currentNumberY] = values[currentNumberX, currentNumberY];
            }
        }
    }

    public static SquareMatrix operator +(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int[,] result = new int[m1.size, m1.size];

        for (int currentNumberX = 0; currentNumberX < m1.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m1.size; ++currentNumberY)
            {
                result[currentNumberX, currentNumberY] = m1.IntMatrix[currentNumberX, currentNumberY] + m2.IntMatrix[currentNumberX, currentNumberY];
            }
        }

        return new SquareMatrix(result);
    }

    public static SquareMatrix operator *(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int[,] result = new int[m1.size, m1.size];

        for (int currentNumberX = 0; currentNumberX < m1.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m1.size; ++currentNumberY)
            {
                for (int k = 0; k < m1.size; ++k)
                {
                    result[currentNumberX, currentNumberY] += m1.IntMatrix[currentNumberX, k] * m2.IntMatrix[k, currentNumberY];
                }
            }
        }

        return new SquareMatrix(result);
    }

    public static bool operator >(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int sum1 = m1.IntMatrix.Cast<int>().Sum();
        int sum2 = m2.IntMatrix.Cast<int>().Sum();

        return sum1 > sum2;
    }

    public static bool operator <(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int sum1 = m1.IntMatrix.Cast<int>().Sum();
        int sum2 = m2.IntMatrix.Cast<int>().Sum();

        return sum1 < sum2;
    }

    public static bool operator >=(SquareMatrix m1, SquareMatrix m2)
    {
        return m1 == m2 || m1 > m2;
    }

    public static bool operator <=(SquareMatrix m1, SquareMatrix m2)
    {
        return m1 == m2 || m1 < m2;
    }

    public static bool operator ==(SquareMatrix m1, SquareMatrix m2)
    {
        if (ReferenceEquals(m1, m2))
        {
            return true;
        }

        if (m1 is null || m2 is null)
        {
            return false;
        }

        if (m1.size != m2.size)
        {
            return false;
        }

        for (int currentNumberX = 0; currentNumberX < m1.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m1.size; ++currentNumberY)
            {
                if (m1.IntMatrix[currentNumberX, currentNumberY] != m2.IntMatrix[currentNumberX, currentNumberY])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool operator !=(SquareMatrix m1, SquareMatrix m2)
    {
        return !(m1 == m2);
    }

    public static explicit operator int(SquareMatrix m)
    {
        int sum = 0;

        for (int currentNumberX = 0; currentNumberX < m.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m.size; ++currentNumberY)
            {
                sum += m.IntMatrix[currentNumberX, currentNumberY];
            }
        }

        return sum;
    }

    public static implicit operator bool(SquareMatrix m)
    {
        for (int currentNumberX = 0; currentNumberX < m.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m.size; ++currentNumberY)
            {
                if (m.IntMatrix[currentNumberX, currentNumberY] != 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int[,] GetMatrix()
    {
        return IntMatrix;
    }

    public double Determinant()
    {
        if (size == 1)
        {
            return IntMatrix[0, 0];
        }

        if (size == 2)
        {
            return IntMatrix[0, 0] * IntMatrix[1, 1] - IntMatrix[0, 1] * IntMatrix[1, 0];
        }

        double det = 0;

        for (int detNumber = 0; detNumber < size; ++detNumber)
        {
            int[,] subMatrix = new int[size - 1, size - 1];

            for (int currentNumberX = 1; currentNumberX < size; ++currentNumberX)
            {
                for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
                {
                    if (currentNumberY < detNumber)
                    {
                        subMatrix[currentNumberX - 1, currentNumberY] = IntMatrix[currentNumberX, currentNumberY];
                    }
                    else if (currentNumberY > detNumber)
                    {
                        subMatrix[currentNumberX - 1, currentNumberY - 1] = IntMatrix[currentNumberX, currentNumberY];
                    }
                }
            }

            det += Math.Pow(-1, detNumber) * IntMatrix[0, detNumber] * new SquareMatrix(subMatrix).Determinant();
        }

        return det;
    }

    public void InverseMatrix()
    {
        int MatrixLength = IntMatrix.GetLength(0);
        double[,] augmentedMatrix = new double[MatrixLength, 2 * MatrixLength];
        for (int currentNumberX = 0; currentNumberX < MatrixLength; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < MatrixLength; ++currentNumberY)
            {
                augmentedMatrix[currentNumberX, currentNumberY] = IntMatrix[currentNumberX, currentNumberY];
                augmentedMatrix[currentNumberX, currentNumberY + MatrixLength] = (currentNumberX == currentNumberY) ? 1 : 0;
            }
        }
        for (int currentNumberX = 0; currentNumberX < MatrixLength; ++currentNumberX)
        {
            double pivot = augmentedMatrix[currentNumberX, currentNumberX];
            if (pivot == 0)
            {
                int swapRow = currentNumberX;
                for (int CurrentNumberY = currentNumberX + 1; CurrentNumberY < MatrixLength; ++CurrentNumberY)
                {
                    if (augmentedMatrix[CurrentNumberY, currentNumberX] != 0)
                    {
                        swapRow = CurrentNumberY;
                        break;
                    }
                }
                if (swapRow == currentNumberX)
                {
                    throw new ArgumentException("The matrix is ​​singular, the inverse matrix does not exist.");
                }
                for (int currentLine = 0; currentLine < 2 * MatrixLength; ++currentLine)
                {
                    double temp = augmentedMatrix[currentNumberX, currentLine];
                    augmentedMatrix[currentNumberX, currentLine] = augmentedMatrix[swapRow, currentLine];
                    augmentedMatrix[swapRow, currentLine] = temp;
                }
                pivot = augmentedMatrix[currentNumberX, currentNumberX];
            }
            for (int currentLine = 0; currentLine < 2 * MatrixLength; ++currentLine)
            {
                augmentedMatrix[currentNumberX, currentLine] /= pivot;
            }
            for (int currentLine = 0; currentLine < MatrixLength; ++currentLine)
            {
                if (currentLine != currentNumberX)
                {
                    double factor = augmentedMatrix[currentLine, currentNumberX];

                    for (int nextLine = 0; nextLine < 2 * MatrixLength; ++nextLine)
                    {
                        augmentedMatrix[currentLine, nextLine] -= factor * augmentedMatrix[currentNumberX, nextLine];
                    }
                }
            }
        }
        double[,] inverseMatrix = new double[MatrixLength, MatrixLength];
        for (int currentNumberX = 0; currentNumberX < MatrixLength; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < MatrixLength; ++currentNumberY)
            {
                inverseMatrix[currentNumberX, currentNumberY] = augmentedMatrix[currentNumberX, currentNumberY + MatrixLength];
            }
        }
        for (int currentNumberX = 0; currentNumberX < MatrixLength; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < MatrixLength; ++currentNumberY)
            {
                Console.Write(inverseMatrix[currentNumberX, currentNumberY] + " ");
            }
            Console.WriteLine();
        }
    }

    public SquareMatrix TransposeMatrix()
    {
        int[,] TransposedMatrix = new int[size, size];
        for (int CurrentNumberX = 0; CurrentNumberX < size; ++CurrentNumberX)
        {
            for (int CurrentNumberY = 0; CurrentNumberY < size; ++CurrentNumberY)
            {
                TransposedMatrix[CurrentNumberX, CurrentNumberY] = IntMatrix[CurrentNumberY, CurrentNumberX];
            }
        }
        return new SquareMatrix(TransposedMatrix);
    }

    public double MatrixTrace()
    {
        if (IntMatrix.GetLength(0) != IntMatrix.GetLength(1))
        {
            throw new ArgumentException("Matrix must be squared.");
        }

        double trace = 0;

        for (int CurrentDiagonalNumber = 0; CurrentDiagonalNumber < size; CurrentDiagonalNumber++)
        {
            trace += IntMatrix[CurrentDiagonalNumber, CurrentDiagonalNumber];
        }

        return trace;
    }

    public static void DiagonalizeMatrix(int[,] matrix)
    {
        if (matrix == null)
        {
            throw new MatrixExistException("Matrix don`t exist.");
        }

        for (int CurrentNumberX = 0; CurrentNumberX < matrix.GetLength(0); CurrentNumberX++)
        {
            for (int CurrentNumberY = 0; CurrentNumberY < matrix.GetLength(0); CurrentNumberY++)
            {
                if (CurrentNumberX != CurrentNumberY)
                {
                    matrix[CurrentNumberX, CurrentNumberY] = 0;
                }
            }
        }
    }


    public override string ToString()
    {
        string result = "";

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                result += IntMatrix[currentNumberX, currentNumberY] + " ";
            }

            result += Environment.NewLine;
        }

        return result;
    }

    public override int GetHashCode()
    {
        int hash = size.GetHashCode();

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                hash ^= IntMatrix[currentNumberX, currentNumberY].GetHashCode();
            }
        }

        return hash;
    }

    public override bool Equals(object obj)
    {
        if (obj is SquareMatrix otherMatrix)
        {
            return Equals(otherMatrix);
        }

        return false;
    }

    public bool Equals(SquareMatrix other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        if (size != other.size)
        {
            return false;
        }

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                if (IntMatrix[currentNumberX, currentNumberY] != other.IntMatrix[currentNumberX, currentNumberY])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int CompareTo(SquareMatrix other)
    {
        if (other is null)
        {
            return 1;
        }

        if (size != other.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int sum1 = IntMatrix.Cast<int>().Sum();
        int sum2 = other.IntMatrix.Cast<int>().Sum();

        return sum1.CompareTo(sum2);
    }

    public object Clone()
    {
        int[,] cloneMatrix = new int[size, size];

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                cloneMatrix[currentNumberX, currentNumberY] = IntMatrix[currentNumberX, currentNumberY];
            }
        }

        return new SquareMatrix(cloneMatrix);
    }
}

class MatrixCalculator
{
    static void Main()
    {

        ChainApplication app = new ChainApplication();
        app.Run(3);

        Console.ReadKey();
        }
    }

