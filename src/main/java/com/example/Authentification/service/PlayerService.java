package com.example.Authentification.service;

import com.example.Authentification.model.Player;
import com.example.Authentification.repository.PlayerRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class PlayerService implements IPlayerService {

    private final PlayerRepository repo;

    public PlayerService(PlayerRepository repo) {
        this.repo = repo;
    }

    @Override
    public Player findByEmail(String email) {
        return repo.findByEmail(email);
    }

    @Override
    @Transactional
    public Player register(String name, String email, String password) {
        if (repo.findByEmail(email) != null) {
            throw new RuntimeException("Email already exists");
        }

        Player player = new Player();
        player.setName(name);
        player.setEmail(email);
        player.setPassword(password);

        return repo.save(player);
    }

    @Override
    public Player createIfNotExists(String email, String name) {
        Player player = repo.findByEmail(email);

        if (player == null) {
            player = new Player();
            player.setEmail(email);
            player.setName(name);
            player.setPassword("");
            player = repo.save(player);
        }

        return player;
    }
}