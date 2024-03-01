--Codigo para la prueba de SQL
CREATE TABLE Clientes (
    ID INT PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50)
);

CREATE TABLE Ventas (
    Fecha DATE,
    Sucursal VARCHAR(50),
    Numero_factura INT,
    Importe DECIMAL(10, 2),
    Id_cliente INT,
    FOREIGN KEY (Id_cliente) REFERENCES Clientes(ID)
);

INSERT INTO Clientes (ID, Nombre, Apellido) VALUES
(1, 'Juan', 'Pérez'),
(2, 'María', 'García'),
(3, 'Luis', 'Martínez'),
(4, 'Ana', 'López');

INSERT INTO Ventas (Fecha, Sucursal, Numero_factura, Importe, Id_cliente) VALUES
('2023-02-28', 'Sucursal A', 1, 25000, 1),
('2023-03-15', 'Sucursal B', 2, 35000, 2),
('2023-04-20', 'Sucursal A', 3, 40000, 3),
('2023-05-10', 'Sucursal C', 4, 20000, 1),
('2023-06-05', 'Sucursal B', 5, 30000, 2),
('2023-07-18', 'Sucursal A', 6, 45000, 3),
('2023-08-22', 'Sucursal C', 7, 50000, 1),
('2023-09-10', 'Sucursal B', 8, 60000, 2),
('2023-10-05', 'Sucursal A', 9, 70000, 3),
('2023-11-12', 'Sucursal C', 10, 30000, 1),
('2023-12-08', 'Sucursal B', 11, 40000, 2),
('2024-01-15', 'Sucursal A', 12, 80000, 3);

--------------------------------------------------------------------------------------------------------
SELECT c.ID, c.Nombre, c.Apellido, SUM(v.Importe) AS TotalCompras
FROM Clientes c
INNER JOIN Ventas v ON c.ID = v.Id_cliente
WHERE v.Fecha >= DATEADD(MONTH, -12, GETDATE())
GROUP BY c.ID, c.Nombre, c.Apellido
HAVING SUM(v.Importe) > 100000;
