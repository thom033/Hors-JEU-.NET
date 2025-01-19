CREATE VIEW equipe_points AS
SELECT 
    e.id_equipe,
    SUM(s.points) AS total_points
FROM 
    Equipes e
JOIN 
    players p ON e.id_equipe = p.id_equipe
JOIN 
    scores s ON p.id_player = s.id_player
GROUP BY 
    e.id_equipe;