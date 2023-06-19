# LinearAlgebra
*A linear algebra library for C#*

The goal of this library is to provide a simple to use implementation of matrices and vectors to perform mathematical operation with C#

```C#
// Simple matrix initialization
Matrix m1 = new Matrix(3,3);
Matrix m2 = new Matrix(3,3);

// Then you can operate with the following operations
var product = m1 * m2;
var sum = m1 + m2;
var inverse = !m1;

// You can retreive single colums and rows by using ranges in the indexer
Vector row1 = m1[1, ..];
Vector column2 = m1[.., 2];
```

The library presents also built-in Gauss-Jordan reduction to solve linear systems
```C#
// Example of computing the inverse using the Gauss method
Matrix m = new Matrix(3,6);
m[0, 0..3] = new Vector(1, 2, 3);
m[1, 0..3] = new Vector(2, 2, 3);
m[2, 0..3] = new Vector(3, 3, 3);

m[.., 3..] = Matrix.Identity(3);

var reduction = Matrix.Gauss(m);

var inverse = reduction[.., 3..];
```
