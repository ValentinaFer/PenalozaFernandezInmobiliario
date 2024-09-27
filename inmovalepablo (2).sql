-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Sep 27, 2024 at 05:01 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmovalepablo`
--

-- --------------------------------------------------------

--
-- Table structure for table `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `inmuebleId` int(11) NOT NULL,
  `inquilinoId` int(11) NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `fechaDesde` datetime NOT NULL DEFAULT current_timestamp(),
  `fechaHasta` datetime NOT NULL,
  `fechaFinalizacion` datetime DEFAULT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `contratos`
--

INSERT INTO `contratos` (`id`, `inmuebleId`, `inquilinoId`, `monto`, `fechaDesde`, `fechaHasta`, `fechaFinalizacion`, `estado`) VALUES
(15, 32, 64, 180000.00, '2024-11-01 00:00:00', '2024-12-31 00:00:00', '2024-12-31 00:00:00', 1),
(16, 32, 64, 180000.00, '2026-02-01 00:00:00', '2026-06-30 00:00:00', '2024-09-26 00:00:00', 1),
(17, 31, 54, 100000.00, '2024-01-01 14:43:48', '2024-05-31 14:43:48', '2024-05-30 14:43:48', 1),
(18, 32, 53, 180000.00, '2025-02-01 00:00:00', '2025-02-28 00:00:00', '2025-02-28 00:00:00', 1),
(19, 32, 56, 180000.00, '2026-01-01 00:00:00', '2026-01-31 00:00:00', NULL, 1),
(20, 32, 56, 180000.00, '2025-01-01 00:00:00', '2025-01-31 00:00:00', NULL, 0),
(21, 32, 64, 100000.00, '2026-03-01 00:00:00', '2026-03-31 00:00:00', NULL, 1),
(22, 31, 62, 200050.00, '2025-05-01 00:00:00', '2025-09-30 00:00:00', NULL, 1),
(23, 31, 62, 200050.00, '2026-05-01 00:00:00', '2026-06-30 00:00:00', NULL, 1);

-- --------------------------------------------------------

--
-- Table structure for table `inmuebles`
--

CREATE TABLE `inmuebles` (
  `idInmueble` int(11) NOT NULL,
  `idPropietario` int(11) NOT NULL,
  `idTipoInmueble` int(11) DEFAULT NULL,
  `uso` varchar(50) NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `direccion` varchar(225) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `superficie` int(11) NOT NULL,
  `latitud` decimal(10,6) NOT NULL,
  `longitud` decimal(10,6) NOT NULL,
  `estado` varchar(50) DEFAULT 'Disponible'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inmuebles`
--

INSERT INTO `inmuebles` (`idInmueble`, `idPropietario`, `idTipoInmueble`, `uso`, `precio`, `direccion`, `ambientes`, `superficie`, `latitud`, `longitud`, `estado`) VALUES
(4, 6, 2, 'Residencial', 2222222.00, 'aqui en mi casa, wey, 123', 2, 162, -24.443196, -12.835292, 'No disponible'),
(27, 6, 4, 'Comercial', 0.00, 'Guayaquil 1953', 5, 500, 100.100000, 2.000000, 'No disponible'),
(28, 6, 2, 'Residencial', 50000.00, 'Guayaquil 1953', 50, 50, -200.100000, -33.200000, 'No disponible'),
(29, 6, 2, 'Comercial', 100020.00, 'Guayaquil 1953', 50, 85, -200.300000, -500.500000, 'No disponible'),
(30, 5, 1, 'Residencial', 100020.00, 'Guayaquil 177', 50, 50, -100.200000, -200.100000, 'Disponible'),
(31, 2, 4, 'Comercial', 200050.00, 'Guayaquil 1953', 10, 40, -33.661966, -65.444490, 'Disponible'),
(32, 6, 2, 'Residencial', 100000.00, 'Granadero Florencio Navarro, D5700 San Luis', 2, 60, -33.316788, -9999.999999, 'Disponible');

-- --------------------------------------------------------

--
-- Table structure for table `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `telefono` varchar(15) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `domicilio` varchar(255) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `nombre`, `apellido`, `email`, `telefono`, `dni`, `domicilio`, `estado`) VALUES
(53, 'Sofia', 'Fernandez', 'quevedoants@gmail.com', '02664122242', '44821639', 'Mi casita, 123', 1),
(54, 'Florence', 'Young', 'florence.young@example.com', '9075274628', '1234521', 'asdasdasd', 1),
(55, 'Leah', 'Miles', 'leah.miles@example.com', '5789919285', '41243153', 'domicilioDeLeah', 1),
(56, 'Sergio', 'Black', 'sergio.black@example.com', '7243293', '33425654', 'domicilioDeSergio', 1),
(57, 'Gabriel', 'O\'Hara', 'GabiOhara@gmail.com', '99192853', '12243425', 'New York, 928', 1),
(58, 'Luis', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '24532611', 'Calle Somewhere 23 altitude or wtever', 1),
(59, 'Oscar', 'Murray', 'oscar.murray@example.com', '(700) 846-3949', '33332344', '6748 White Oak Dr', 1),
(60, 'Clifford', 'Neal', 'clifford.neal@example.com', '(776) 753-2578', '30753753', '7464 Taylor St', 1),
(61, 'Mae', 'Holland', 'mae.holland@example.com', '(201) 423-4752', '33423752', '4383 Royal Ln', 1),
(62, 'Willie', 'Curtis', 'willie.curtis@example.com', '9338630043', '36303043', '4038 Nowlin Rd', 1),
(63, 'miriam', 'Franklin', 'miriam.franklin@example.com', '(845) 606-6121', '27121660', '434 E Sandy Lake Rd', 1),
(64, 'Jonathan Dario', 'Fernandez', 'JonathanFer@gmail.com', '2664450142', '36748424', 'Granadero 15 alt', 1),
(66, 'Gwen', 'Miles', 'asdf@sdfsdf.com', '5789919285', '34444444', 'Calle Somewhere 23 altitude or wtever', 0);

-- --------------------------------------------------------

--
-- Table structure for table `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `idContrato` int(11) NOT NULL,
  `nroPago` int(5) NOT NULL,
  `fechaPago` datetime NOT NULL DEFAULT current_timestamp(),
  `detallePago` varchar(100) NOT NULL,
  `importe` decimal(10,2) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pagos`
--

INSERT INTO `pagos` (`id`, `idContrato`, `nroPago`, `fechaPago`, `detallePago`, `importe`, `estado`) VALUES
(36, 15, 1, '2024-09-26 00:00:00', 'Primer Pago, Noviembre, 2024', 180000.00, 1),
(37, 15, 2, '2024-09-26 00:00:00', 'Segundo Pago, Diciembre, 2024', 180000.00, 1),
(38, 16, 1, '2024-09-26 00:00:00', 'Primer Pago, Febrero, 2026', 180000.00, 1),
(39, 16, 2, '2024-09-26 00:00:00', 'Segundo Pago, Marzo, 2026', 180000.00, 1),
(40, 18, 1, '2024-09-26 00:00:00', 'Primer pago, Mes de Febrero, 2025(test)', 180000.00, 0),
(44, 18, 1, '2024-09-26 00:00:00', 'Primer Pago, Febrero, 2025', 180000.00, 1),
(54, 17, 1, '2024-09-26 00:00:00', 'Primer Pago', 100000.00, 1);

-- --------------------------------------------------------

--
-- Table structure for table `propietarios`
--

CREATE TABLE `propietarios` (
  `idPropietario` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `telefono` varchar(15) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `domicilio` varchar(255) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `propietarios`
--

INSERT INTO `propietarios` (`idPropietario`, `nombre`, `apellido`, `email`, `telefono`, `dni`, `domicilio`, `estado`) VALUES
(1, 'Luis', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '24452411235', 'domiciliasd, algoNose 12', 0),
(2, 'Valentina Sofia', 'Fernandez', 'quevedoants@gmail.com', '02664860342', '24452411235', 'domiciliasd, algoNose 12', 1),
(3, 'Florence', 'Franklin', 'florence.young@example.com', '9075274628', '27121660', '434 E Sandy Lake Rd', 1),
(4, 'Darrell', 'Hughes', 'darrell.hughes@example.com', '(659) 635-9518', '34518518', '3304 Ranchview Dr', 1),
(5, 'Pablo', 'Peñaloza', 'panlo@gmail.com', '2657302496', '34518519', 'Guayaquil1864', 1),
(6, 'Cecilia Fanny', 'Aguilar', 'panlo@gmail.com', '02657302496', '4444', 'Guayaquil1864', 1);

-- --------------------------------------------------------

--
-- Table structure for table `tiposinmueble`
--

CREATE TABLE `tiposinmueble` (
  `id` int(11) NOT NULL,
  `tipo` varchar(50) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tiposinmueble`
--

INSERT INTO `tiposinmueble` (`id`, `tipo`, `estado`) VALUES
(1, 'Departamento', 1),
(2, 'Casa', 1),
(3, 'Depósito', 1),
(4, 'Local', 1);

-- --------------------------------------------------------

--
-- Table structure for table `usuarios`
--

CREATE TABLE `usuarios` (
  `IdUsuario` int(11) NOT NULL,
  `Nombre` varchar(255) NOT NULL,
  `Apellido` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Clave` varchar(255) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL,
  `Rol` int(11) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usuarios`
--

INSERT INTO `usuarios` (`IdUsuario`, `Nombre`, `Apellido`, `Email`, `Clave`, `Avatar`, `Rol`, `estado`) VALUES
(21, 'Pablo', 'Peñaloza', 'Pablonicolasask@gmail.com', 'itlJGxmPFckOhS3cZfjiAlY0KYGHoEYGmF5XZjpWPxQ=', '/avatars/hombre_20240920165728.png', 1, 1),
(22, 'pedro', 'sanchez', 'JuanCarlos@gmail.com', 'itlJGxmPFckOhS3cZfjiAlY0KYGHoEYGmF5XZjpWPxQ=', '/avatars/hombre_20240920170203.png', 2, 1),
(25, 'Valentina Sofia', 'Fernandez', 'quevedoants@gmail.com', 'yosZNJUPHEWs07e4Evz0PKElSB7/r+BrfB2qlzTQbYQ=', '/avatars/wallpaper3_20240925232222.png', 1, 1),
(26, 'Valentina Sofia', 'Fernandez', 'valee@gmail.com', 'yosZNJUPHEWs07e4Evz0PKElSB7/r+BrfB2qlzTQbYQ=', '/avatars/wallpaper3_20240925232320.png', 2, 0),
(27, 'Florence', 'Young', 'Florence@gmail.com', 'o0j9H30BPG6lMcabxI/GJ5CLdqRRXtV6oFrbUsAJUYE=', '', 2, 1);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `inquilinoId` (`inquilinoId`),
  ADD KEY `contratos_ibfk_2` (`inmuebleId`);

--
-- Indexes for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`idInmueble`),
  ADD KEY `idPropietario` (`idPropietario`) USING BTREE,
  ADD KEY `idTipoInmueble` (`idTipoInmueble`) USING BTREE;

--
-- Indexes for table `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idContrato` (`idContrato`);

--
-- Indexes for table `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`idPropietario`);

--
-- Indexes for table `tiposinmueble`
--
ALTER TABLE `tiposinmueble`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `tipo` (`tipo`);

--
-- Indexes for table `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`IdUsuario`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT for table `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `idInmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- AUTO_INCREMENT for table `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=69;

--
-- AUTO_INCREMENT for table `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=57;

--
-- AUTO_INCREMENT for table `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `idPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `tiposinmueble`
--
ALTER TABLE `tiposinmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`inquilinoId`) REFERENCES `inquilinos` (`id`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`inmuebleId`) REFERENCES `inmuebles` (`idInmueble`);

--
-- Constraints for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`idTipoInmueble`) REFERENCES `tiposinmueble` (`id`),
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`idPropietario`) REFERENCES `propietarios` (`idPropietario`);

--
-- Constraints for table `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`idContrato`) REFERENCES `contratos` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
