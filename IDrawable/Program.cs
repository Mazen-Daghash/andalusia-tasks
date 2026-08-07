using System;

interface IDrawable
{
    void Draw();
}

abstract class Shape
{
    public abstract double Area();

    public void Describe()
    {
        Console.WriteLine($"{GetType().Name} - Area: {Area():F2}");
    }
}

class Circle : Shape, IDrawable
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double Area()
    {
        return Math.PI * Radius * Radius;
    }

    public void Draw()
    {
        Console.WriteLine("   o   ");
    }
}

class Rectangle : Shape, IDrawable
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double Area()
    {
        return Width * Height;
    }

    public void Draw()
    {
        Console.WriteLine("[   ]");
        Console.WriteLine("[   ]");
    }
}

class Triangle : Shape, IDrawable
{
    public double Base { get; set; }
    public double Height { get; set; }

    public Triangle(double baseLength, double height)
    {
        Base = baseLength;
        Height = height;
    }

    public override double Area()
    {
        return 0.5 * Base * Height;
    }

    public void Draw()
    {
        Console.WriteLine("  /\\  ");
        Console.WriteLine(" /  \\ ");
        Console.WriteLine("/____\\");
    }
}

class Program
{
    static void Main()
    {
        Shape[] shapes = new Shape[]
        {
            new Circle(3),
            new Rectangle(4, 5),
            new Triangle(6, 4)
        };

        foreach (var shape in shapes)
        {
            shape.Describe();
            ((IDrawable)shape).Draw();
            Console.WriteLine();
        }
    }
}