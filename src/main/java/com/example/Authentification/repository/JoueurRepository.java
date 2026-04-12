package com.example.Authentification.repository;

import com.example.Authentification.model.Joueur;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface JoueurRepository extends JpaRepository<Joueur, Long> {
    boolean existsByEmail(String email);
    Optional<Joueur> findByEmail(String email);
}
