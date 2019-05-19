import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import axios from 'axios';
import { APIService } from '../../APIService';
const apiService = new APIService();
import VueRouter from 'vue-router';
Vue.use(VueRouter);


interface movie {
    title: string;
    year: string;
    rated: string;
    actors: string;
    awards: string;
    ratings: ratings[];
    error: string;
    response: boolean;
    poster: string;
}


interface ratings {
    id: number;
    source: string;
    value: string;
    movieId: number;
}

@Component
export default class MovieDetailComponent extends Vue {

    token: string = localStorage.getItem('token') || '';
    loading: boolean = false;
    noData: boolean = false;
    $router: any;
    imdbId: string = "";
    movie: movie = <movie>{
        title: "",
        year: "",
        rated: "",
        actors: "",
        awards: "",
        ratings: [],
        response: false,
        poster: "",
        error: ""
    };
    $route: any;

    back() {
        this.$router.push('/')
    }


    getMovie() {
        this.imdbId = this.$route.params.imdbId;
        this.loading = true;
        axios({
            method: 'get',
            url: apiService.API_URL + '/api/Movie/SearchMovieByImdbId?imdbId=' + this.imdbId,
            headers: {
                Authorization: 'Bearer ' + this.token,
                'Content-Type': 'application/json'
            }
            }).then((response: any) => {
                this.movie = response.data;
                if (this.movie.response == true) {
                    this.loading = false;
                    this.noData = false;
                }
                else {
                    this.noData = true;
                    this.loading = false;
                }
            })
            .catch((error: any) => {
                debugger;
                console.log(error);
                if (error.response.status === 401) {
                    this.$router.push('/logon');
                } 
            });
    }

    mounted() {
        this.getMovie();
    }

}