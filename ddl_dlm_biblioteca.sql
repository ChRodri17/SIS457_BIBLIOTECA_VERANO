CREATE DATABASE Biblioteca;

USE master
GO
CREATE LOGIN usrbiblioteca WITH PASSWORD=N'123456',
	DEFAULT_DATABASE=Biblioteca,
	CHECK_EXPIRATION=OFF,
	CHECK_POLICY=ON
GO
USE Biblioteca
GO
CREATE USER usrbiblioteca FOR LOGIN usrbiblioteca
GO
ALTER ROLE db_owner ADD MEMBER usrbiblioteca
GO


DROP TABLE Producto;
DROP TABLE Categoria;
DROP TABLE Cliente;
DROP TABLE Empleado;
DROP TABLE Usuario;
DROP TABLE Venta;
DROP TABLE VentaDetalle;


CREATE TABLE Categoria(
  idCategoria INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  nombre VARCHAR(30) NOT NULL
);

CREATE TABLE Cliente(
  id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  razonSocial VARCHAR(50) NOT NULL,
  cedulaIdentidad VARCHAR(15) NOT NULL,
  celular VARCHAR(8) NOT NULL DEFAULT '0',
);

CREATE TABLE Empleado(
  idEmpleado INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  nombre VARCHAR(20) NOT NULL,
  apellidos VARCHAR(20) NOT NULL,
  telefono VARCHAR(20) NOT NULL,
  cargo VARCHAR(20) NOT NULL,
  salario FLOAT NOT NULL,
);

CREATE TABLE Usuario(
  idUsuario INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  idEmpleado INT NOT NULL,
  usuario VARCHAR(30) NOT NULL,
  clave VARCHAR(30) NOT NULL,
  FOREIGN KEY (idEmpleado) REFERENCES Empleado(idEmpleado)
);

CREATE TABLE Producto(
  idProducto INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  idCategoria INT NOT NULL,
  codigo VARCHAR(20) NOT NULL,
  nombre VARCHAR(50) NOT NULL,
  descripcion VARCHAR(500) NOT NULL,
  precio DECIMAL (5,2) NOT NULL,
  FOREIGN KEY (idCategoria) REFERENCES Categoria(idCategoria)
);

CREATE TABLE Venta(
  id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  idUsuario INT NOT NULL,
  idCliente INT NOT NULL,
  totalVenta DECIMAL NOT NULL,
  fechaVenta DATE NOT NULL DEFAULT GETDATE(),
  CONSTRAINT fk_Venta_Usuario FOREIGN KEY(idUsuario) REFERENCES Usuario(idUsuario),
  CONSTRAINT fk_Venta_Cliente FOREIGN KEY(idCliente) REFERENCES Cliente(id)
);

CREATE TABLE VentaDetalle(
  id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
  idVenta INT NOT NULL,
  idProducto INT NOT NULL,
  cantidad INT NOT NULL CHECK(cantidad > 0),
  precioUnitario DECIMAL NOT NULL,
  total DECIMAL NOT NULL,
  CONSTRAINT fk_VentaDetalle_Venta FOREIGN KEY(idVenta) REFERENCES Venta(id),
  CONSTRAINT fk_VentaDetalle_Producto FOREIGN KEY(idProducto) REFERENCES Producto(idProducto)
);

--Eliminación lógica

ALTER TABLE Empleado ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Empleado ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Empleado ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Usuario ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Usuario ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Usuario ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Categoria ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Categoria ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Categoria ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Producto ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Producto ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Producto ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Cliente ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Cliente ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Cliente ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Venta ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Venta ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Venta ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE VentaDetalle ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE VentaDetalle ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE VentaDetalle ADD estado SMALLINT NOT NULL DEFAULT 1;

--Procedimientos Almacenados

CREATE PROC paEmpleadoListar @parametro VARCHAR(50)
AS
  SELECT idEmpleado,nombre,apellidos,telefono,cargo,salario
  FROM Empleado
  WHERE estado<>-1 AND nombre LIKE '%'+REPLACE(@parametro,' ','%')+'%';

CREATE PROCEDURE paProductosListar @parametro VARCHAR(50)
AS
  SELECT p.*, c.nombre AS categoria
  FROM producto AS p
  INNER JOIN categoria AS c ON p.idCategoria = c.idCategoria
  WHERE p.estado<>-1 AND p.nombre LIKE '%'+REPLACE(@parametro,' ','%')+'%';

  CREATE PROC paClienteListar @parametro VARCHAR(50)
AS 
  SELECT *
  FROM Cliente
  WHERE estado<>-1 AND razonSocial LIKE '%'+REPLACE(@parametro,' ','%');

CREATE PROC paVentaListar @parametro VARCHAR(50)
AS 
  SELECT v.*, u.usuario, c.razonSocial
  FROM Venta v
  INNER JOIN Usuario u ON u.idUsuario = v.idUsuario
  INNER JOIN Cliente c ON c.id = v.idCliente 
  WHERE v.estado<>-1 AND c.razonSocial LIKE '%'+REPLACE(@parametro,' ','%')+'%';

CREATE PROC paVentaDetalleListar @parametro VARCHAR(50)
AS 
  SELECT vd.*, p.nombre, c.razonSocial
  FROM VentaDetalle vd
  INNER JOIN Producto p ON p.idProducto = vd.idProducto
  INNER JOIN Venta v ON v.id = vd.idVenta
  INNER JOIN Cliente c ON c.id = v.idCliente 
  WHERE vd.estado<>-1 AND c.razonSocial LIKE '%'+REPLACE(@parametro,' ','%')+'%';

INSERT INTO Categoria(nombre)
VALUES ('Cuentos');

INSERT INTO Producto(idCategoria,codigo,nombre,descripcion,precio)
VALUES (1, 'C123', 'Blanca Nieves y los siete Enanos', 'Una bella princesa, una celosa madrastra y una manzana envenenada.', 15.50);

INSERT INTO Empleado (nombre, apellidos, telefono, cargo, salario)
VALUES ('Juan', 'Perez', '554433', 'Gerente', 5000);

INSERT INTO Usuario (idEmpleado, usuario, clave) 
VALUES (1, 'jperez', 'sis457');

SELECT * FROM Usuario;

SELECT U.*, E.nombre 
FROM Usuario U 
INNER JOIN Empleado E
    ON U.idEmpleado = E.idEmpleado;


INSERT INTO Cliente (razonSocial, cedulaIdentidad, celular)
VALUES ('Colegio Monteagudo', '8002563', '7504339');

