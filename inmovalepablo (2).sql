-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 09-04-2024 a las 22:09:11
-- Versión del servidor: 10.4.28-MariaDB
-- Versión de PHP: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmovalepablo`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `superficie` int(11) NOT NULL,
  `latitud` decimal(10,6) DEFAULT NULL,
  `longitud` decimal(10,6) DEFAULT NULL,
  `propietarioId` int(11) DEFAULT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `direccion`, `ambientes`, `superficie`, `latitud`, `longitud`, `propietarioId`, `estado`) VALUES
(1, 'Calle 95, #397', 1, 162, -24.443196, -132.835292, 3, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `dni` varchar(20) DEFAULT NULL,
  `domicilio` varchar(255) DEFAULT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `nombre`, `apellido`, `email`, `telefono`, `dni`, `domicilio`, `estado`) VALUES
(1, 'Valentina', 'Fernandez', 'quevedoants@gmail.com', '2664860342', '44530625', 'Barrio Maximiliano toro, mza i, casa 15', 0),
(2, 'Sophie', 'Gonzales', 'SophGonz@gmail.com', '2664846372', '44531627', 'Somewhere, street 123', 0),
(9, 'Florence', 'Young', 'florence.young@example.com', '9075274628', '333333333', 'domicilioDeFlorence', 0),
(10, 'Luis', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '33666666', 'Calle Somewhere 23 altitude or wtever', 0),
(11, 'Alguien', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '24452411235', 'domiciliasd, algoNose 12', 0),
(12, 'Sergio', 'Black', 'sergio.black@example.com', '(853) 776-7044', '666666666', '8407 James St', 0),
(13, 'Connor', 'Brewer', 'connor.brewer@example.com', '(815) 444-3010', 'i0104481301', '3669 Hamilton Ave', 0),
(14, 'Terrance', 'Willis', 'terrance.willis@example.com', '(777) 769-6917', 'M1776769', '2518 Miller Ave', 0),
(15, 'Scott', 'Douglas', 'scott.douglas@example.com', '(516) 841-9606', 'P96968416', '5479 Paddock Way', 0),
(16, 'Felix', 'Lewis', 'felix.lewis@example.com', '(672) 599-1256', '123112315', '7993 Timber Wolf Trail', 0),
(17, 'Jill', 'Dixon', 'jill.dixon@example.com', '(400) 559-1111', '559098117', '4798 Pockrus Page Rd', 0),
(18, 'Leah', 'Miles', 'leah.miles@example.com', '(578) 991-9285', '789128285', '8970 White Oak Dr', 0),
(19, 'Sofia', 'Mitchelle', 'sofia.mitchelle@example.com', '(544) 828-7391', '739191738', '4520 Robinson Rd', 0),
(20, 'Brayden', 'Green', 'brayden.green@example.com', '(794) 680-4670', '470670808', '8694 Mcclellan Rd', 0),
(21, 'Miguel', 'O\'Hara', 'sad@saddd', '5789919285', '123123', 'New York, 928', 0),
(22, 'Miles', 'Morales', 'MoralesMiles@gmail', '02664860342', '123123', 'Brooklyn, 1610', 0),
(23, 'Gabriel', 'O\'Hara', 'GabrielOhara@gmail', '9919285', '321321321', 'domicilioDeEsteWey', 0),
(24, 'Gabriel', 'O\'Hara', 'asd@fsafsda', '9919285', 'a1222222', 'domicilioDeEsteWey', 0),
(25, 'Gabriel', 'O\'Hara', 'daafffdsdf@ggfddf', '9919285', 'a1222222', 'domicilioDeEsteWey', 0),
(26, 'Valentina Sofia', 'Fernandez', 'quevedoants@gmail.com', '02664860342', '55643846', 'New York, 928', 0),
(27, 'Valentina Sofia', 'Fernandez', 'asdf@asdfg', '02664860342', '55643846', 'New York, 928', 0),
(28, 'Gwen', 'Miles', 'leah.miles@example.com', '5789919285', '34444444', 'Calle Somewhere 23 altitude or wtever', 0),
(29, 'Gwen', 'Miles', 'asdf@sdfsdf', '5789919285', '34444444', 'Calle Somewhere 23 altitude or wtever', 0),
(30, 'Gabriel', 'O\'Hara', 'daafffdsdf@ggfddf', '9919285', '123123', 'asdasdasd', 0),
(31, 'Gabriel', 'O\'Hara', 'hgfdsa@adfsdsf', '9919285', '123123', 'asdasdasd', 0),
(32, 'Gabriel', 'O\'Hara', 'fgsdfg@gdfg', '9919285', '123123', 'asdasdasd', 0),
(33, 'Gabriel', 'O\'Hara', 'AAAAAAAAA@a', '9919285', '123123', 'asdasdasd', 0),
(34, 'Gabriel', 'O\'Hara', 'dfsfds@adasd', '9919285', '123123', 'asdasdasd', 0),
(35, 'Gabriel', 'O\'Hara', 'gggggggg@matenme', '9919285', '123123', 'asdasdasd', 0),
(36, 'Miguelito', 'O\'Hara', 'Miguelito@gmail', '9919285', '123123', 'asdasdasd', 0),
(37, 'Miguelito', 'O\'Hara', 'afd@sfasdfasdf', '9919285', '123123', 'asdasdasd', 0),
(38, 'Miguelito', 'O\'Hara', 'asfd@afd', '9919285', '123123', 'asdasdasd', 0),
(39, 'Miguelito', 'O\'Hara', 'sdfafsf@sfsfd', '9919285', '123123', 'New York, 928', 0),
(40, 'Miguelito', 'O\'Hara', 'sdfafsf@sfsfd', '9919285', '123123', 'New York, 928', 0),
(41, 'Miguelito', 'O\'Hara', 'sdfafsf@sfsfd', '9919285', '123123', 'New York, 928', 0),
(42, 'Miguelito', 'O\'Hara', 'sdfafsf@sfsfd', '9919285', '123123', 'New York, 928', 0),
(43, 'juangm', 'carlos', 'pp@gmail.com', '22222', '422222', 'guayaquil 1864', 0),
(44, 'juangm', 'carlos', 'pp@gmail.com', '22222', '422222', 'guayaquil 1864', 0),
(45, 'juangm', 'carlos', 'pp@gmail.com', '22222', '422222', 'guayaquil', 0),
(46, 'juangm', 'carlos', 'pp@gmail.com', '22222', '422222', 'guayaquil 1864', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `idPropietario` int(11) NOT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `dni` varchar(20) DEFAULT NULL,
  `domicilio` varchar(255) DEFAULT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`idPropietario`, `nombre`, `apellido`, `email`, `telefono`, `dni`, `domicilio`, `estado`) VALUES
(1, 'Luis', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '24452411235', 'domiciliasd, algoNose 111', 0),
(2, 'asdsad', 'asdasd', 'asdasd@gmail.com', '222222222222', '22222', 'Guayaquil1864', 0),
(3, 'pablo', 'Peñaloza', 'asdasd@gmail.com', '222222222222', '22222', 'domiciliasd, algoNose 10', 0),
(4, 'asdasd', 'asdasd', 'asdasd@gmail.com', '222222222222', '333333333333333333', 'domiciliasd, algoNose 111', 0),
(5, 'asdsad', 'asdasd', 'asdasd@gmail.com', '222222222222', '3444444444', 'domiciliasd, algoNose 111', 0),
(6, 'asdasd', 'asdasd', 'asdasd@gmail.com', '222222222222', '4424', 'Guayaquil186433', 0),
(7, 'asdsad', 'asdasd', 'asdasd@gmail.com', '222222222222', '4424333', 'Guayaquil1864', 0),
(8, 'asdsad', 'asdasd', 'asdasd@gmail.com', '3333333', '4424333222', 'Guayaquil186433', 0),
(9, 'asdsad', 'asdasd', 'asdasd@gmail.com', '44444', '33333333333333333334', '44444', 0),
(10, 'asdsad', 'asdasd', 'asdasd@gmail.com', '3333333', '333333', '444444222', 0),
(11, 'asdsad', 'asdasd', 'asdasd@gmail.com', '3333333', '442442', 'Guayaquil186433', 0),
(12, 'asdsad', 'asdasd', 'asdasd@gmail.com', '3333333', '3444444444', 'domiciliasd, algoNose 10', 0),
(13, 'asdasd', 'asdasd', 'asdasd@gmail.com', '222222222222', '4424333', 'domiciliasd, algoNose 111', 0),
(14, 'asdsad', 'asdasd', 'asdasd@gmail.com', '44444', '333333333333333333', 'domiciliasd, algoNose 10', 0),
(15, 'asdsad', 'asdasd', 'asdasd@gmail.com', '222222222222', '4424333', 'Guayaquil1864', 0),
(16, 'asdsad', 'asdasd', 'asdasd@gmail.com', '44444', '4424', 'domiciliasd, algoNose 111', 0),
(17, 'asdsad', 'asdasd', 'asdasd@gmail.com', '44444', '333333333333333333', 'domiciliasd, algoNose 10', 0),
(18, 'dfsfgdg', 'rrrrr', 'eeeeee@gmail.com', '33434343434', '33334444', 'Guayaquil186433', 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_propietario_inmueble` (`propietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`idPropietario`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `idPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `fk_propietario_inmueble` FOREIGN KEY (`propietarioId`) REFERENCES `propietarios` (`idPropietario`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
