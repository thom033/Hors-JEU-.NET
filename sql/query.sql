CREATE OR REPLACE VIEW equipe_points AS
SELECT equipe_name, SUM(points) AS total_points
FROM valiny
GROUP BY equipe_name;

CREATE OR REPLACE VIEW player_points AS
SELECT id_player, equipe_name, SUM(points) AS total_points
FROM valiny
GROUP BY id_player, equipe_name;

CREATE OR REPLACE VIEW equipe_points AS
SELECT equipe_name, SUM(points) AS total_points
FROM valiny
GROUP BY equipe_name;

CREATE OR REPLACE VIEW equipe_arret AS 
SELECT equipe_name, SUM(arret) AS total_arret 
FROM valiny WHERE arret IS NOT NULL 
GROUP BY equipe_name;
