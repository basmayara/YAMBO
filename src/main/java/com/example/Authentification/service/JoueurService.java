package com.example.Authentification.service;

import com.example.Authentification.dto.RegisterDTO;
import com.example.Authentification.model.Joueur;
import com.example.Authentification.repository.JoueurRepository;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

@Service
public class JoueurService {

    private final JoueurRepository joueurRepository;
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    public JoueurService(JoueurRepository joueurRepository) {
        this.joueurRepository = joueurRepository;
    }

    public Joueur inscrire(RegisterDTO dto) {
        if (joueurRepository.existsByEmail(dto.getEmail())) {
            throw new RuntimeException("Cet email est déjà utilisé");
        }

        Joueur joueur = new Joueur();
        joueur.setNom(dto.getNom());
        joueur.setEmail(dto.getEmail());
        joueur.setMotDePasse(passwordEncoder.encode(dto.getMotDePasse()));
        joueur.setNiveau(1);
        joueur.setScoreTotal(0);

        return joueurRepository.save(joueur);
    }
    public Joueur findByEmail(String email) {
        return joueurRepository.findByEmail(email)
                .orElseThrow(() -> new RuntimeException("User not found"));
    }
}
