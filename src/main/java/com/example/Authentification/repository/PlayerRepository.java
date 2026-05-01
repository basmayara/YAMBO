package com.example.Authentification.repository;

import com.example.Authentification.model.Player;
import org.springframework.data.jpa.repository.JpaRepository;

public interface PlayerRepository extends JpaRepository<Player, Long> {
    Player findByEmail(String email);
}