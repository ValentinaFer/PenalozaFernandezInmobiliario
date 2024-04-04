-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 04, 2024 at 06:04 PM
-- Server version: 10.4.24-MariaDB
-- PHP Version: 8.1.6

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

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `GET_HASMOREPAGES` (IN `TableName` VARCHAR(100), IN `PageNumber` INT, IN `PageSize` INT, OUT `HasMorePages` BIT)   BEGIN 

SET @SqlQuery = CONCAT('SELECT COUNT(*) INTO @TotalCount FROM ', TableName, ' WHERE ', TableName,'.estado=1');
PREPARE stmt FROM @SqlQuery;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

IF (pageNumber * PageSize)< @TotalCount THEN
	SET HasMorePages = TRUE;
ELSE 
	SET HasMorePages = FALSE;
END IF;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `inquilinos`
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `nombre`, `apellido`, `email`, `telefono`, `dni`, `domicilio`, `estado`) VALUES
(53, 'Valentina Sofia', 'Fernandez', 'quevedoants@gmail.com', '02664122242', '44821639', 'Mi casita, 123', 1),
(54, 'Florence', 'Young', 'florence.young@example.com', '9075274628', 'a1234521', 'asdasdasd', 1),
(55, 'Leah', 'Miles', 'leah.miles@example.com', '5789919285', '41243153', 'domicilioDeLeah', 1),
(56, 'Sergio', 'Black', 'sergio.black@example.com', '7243293', '33425654', 'domicilioDeSergio', 1),
(57, 'Gabriel', 'O\'Hara', 'GabiOhara@ggfddf', '99192853', 'a12243425', 'New York, 928', 1),
(58, 'Luis', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '24532611', 'Calle Somewhere 23 altitude or wtever', 1),
(59, 'Oscar', 'Murray', 'oscar.murray@example.com', '(700) 846-3949', '33332344', '6748 White Oak Dr', 1),
(60, 'Clifford', 'Neal', 'clifford.neal@example.com', '(776) 753-2578', '30753753', '7464 Taylor St', 1),
(61, 'Mae', 'Holland', 'mae.holland@example.com', '(201) 423-4752', '33423752', '4383 Royal Ln', 1),
(62, 'Willie', 'Curtis', 'willie.curtis@example.com', '(933) 863-0043', '36303043', '4038 Nowlin Rd', 1),
(63, 'miriam', 'Franklin', 'miriam.franklin@example.com', '(845) 606-6121', '27121660', '434 E Sandy Lake Rd', 1);

-- --------------------------------------------------------

--
-- Table structure for table `propietarios`
--

CREATE TABLE `propietarios` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `dni` varchar(20) DEFAULT NULL,
  `domicilio` varchar(255) DEFAULT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `propietarios`
--

INSERT INTO `propietarios` (`id`, `nombre`, `apellido`, `email`, `telefono`, `dni`, `domicilio`, `estado`) VALUES
(1, 'Luis', 'Rodriguez', 'luis.rodriguez@example.com', '80606534', '24452411235', 'domiciliasd, algoNose 12', 0),
(2, 'Valentina Sofia', 'Fernandez', 'quevedoants@gmail.com', '02664860342', '24452411235', 'domiciliasd, algoNose 12', 1),
(3, 'Florence', 'Franklin', 'florence.young@example.com', '9075274628', '27121660', '434 E Sandy Lake Rd', 1),
(4, 'Darrell', 'Hughes', 'darrell.hughes@example.com', '(659) 635-9518', '34518518', '3304 Ranchview Dr', 1);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=64;

--
-- AUTO_INCREMENT for table `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
